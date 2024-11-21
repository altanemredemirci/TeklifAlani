using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using TeklifAlani.Core.DTOs.ApplicationUserDTO;
using TeklifAlani.Core.Identity;
using TeklifAlani.Core.Models;
using TeklifAlani.WebUI.Models;
using TeklifAlani.WebUI.EmailServices;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace TeklifAlani.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly string _apiBaseUrl;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _apiBaseUrl = configuration["ApiSettings:ApiUrl"];
        }

        public async Task<IActionResult> Register()
        {

            var apiUrl = $"{_apiBaseUrl}/Values/GetCities";
            var response = await _httpClient.GetStringAsync(apiUrl);

            // Gelen JSON'u deserialize ederek döndürüyoruz
            var results = JsonSerializer.Deserialize<List<City>>(response);

            ViewData["cities"] = results;

            return View(new CreateApplicationUserDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateApplicationUserDTO model, IFormFile file)
        {
            if (file == null)
            {
                ModelState.AddModelError("", "Lütfen Firma Logonuzu Yükleyiniz.");
            }
            else
            {
                ModelState.Remove("Logo");
                ModelState.Remove("Username");

                if (ModelState.IsValid)
                {
                    // Logoyu işleyin ve kullanıcıyı API'ye gönderin
                    var postResponse = await RegisterUserAndUploadLogoAsync(model, file);

                    if (postResponse.IsSuccessStatusCode)
                    {
                        var user = await _userManager.FindByEmailAsync(model.Email);
                        if (user != null)
                        {
                            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                new { userId = user.Id, token = token }, Request.Scheme);

                            var body = $"Lütfen e-posta adresinizi doğrulamak için bu bağlantıya tıklayın: <a href='{confirmationLink}'>E-postayı Doğrula</a>";

                            // Mail gönderimi
                            await _emailSender.SendEmailAsync(user.Email, "E-posta Doğrulama", body);
                        }

                        TempData["message"] = "Lütfen email adresinize gönderilen link ile hesabınızı onaylayınız";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        var errorContent = await postResponse.Content.ReadAsStringAsync();
                        AddErrorsToModelState(errorContent);
                    }
                }
            }

            await LoadCitiesAsync();
            return View(model);
        }

        public async Task<IActionResult> Login(string ReturnUrl = null)
        {
            return View(new LoginApplicationUserDTO()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginApplicationUserDTO model)
        {
            ModelState.Remove("ReturnUrl");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

            if (user == null)
            {
                ModelState.AddModelError("", "Bu telefon numarası ile hesap oluşturulmamıştır.");
                return NotFound(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Lütfen hesabınızı email ile onaylayınız.");
                return BadRequest(model);
            }

            // Şifre ile giriş yap
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);

            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/");
            }

            ModelState.AddModelError("", "Giriş başarısız. Lütfen bilgilerinizi kontrol edin.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["message"] = "Hesabınız Onaylandı.";
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        #region Düzelt
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ResetPassword", "Account",
                              new { email = user.Email, token = code }, Request.Scheme);

            var body = $"Parolanızı yenilemek için lütfen <a href='{callbackUrl}'> linke tıklayınız.</a>";

            // Mail gönderimi
            await _emailSender.SendEmailAsync(user.Email, "Yeni Şifre Oluşturma", body);
            TempData["message"] = "Şifrenizi yenilemek için lütfen email adresinize gönderilen linke tıklayınız";
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ResetPasswordModel() { Token = token, Email = email };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                TempData["message"] = "Yeni şifreniz ile giriş yapabilirsiniz.";
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion

        private async Task LoadCitiesAsync()
        {
            var apiUrl = $"{_apiBaseUrl}/Values/GetCities";
            var response = await _httpClient.GetStringAsync(apiUrl);
            var results = JsonSerializer.Deserialize<List<City>>(response);
            ViewData["cities"] = results;
        }

        private async Task<HttpResponseMessage> RegisterUserAndUploadLogoAsync(CreateApplicationUserDTO model, IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (Image image = Image.Load(stream.ToArray()))
                {
                    image.Mutate(x => x.Resize(128, 128));
                    var fileName = $"{model.PhoneNumber}.webp";
                    model.Logo = fileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);
                    await image.SaveAsync(filePath, new WebpEncoder());
                }
            }

            model.UserName = model.Email;
            var jsonContent = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var applicationUrl = $"{_apiBaseUrl}/User/RegisterUser";
            return await _httpClient.PostAsync(applicationUrl, content);
        }

        private void AddErrorsToModelState(string errorContent)
        {
            var errors = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(errorContent);
            foreach (var error in errors.SelectMany(e => e.Value))
            {
                ModelState.AddModelError("", error);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult IsSessionActive()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return Ok(); // Kullanıcı oturum açmış
            }
            return Unauthorized(); // Kullanıcı oturum açmamış
        }

    }
}
