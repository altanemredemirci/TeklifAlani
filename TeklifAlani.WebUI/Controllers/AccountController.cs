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
using TeklifAlani.BLL.Abstract;
using AutoMapper;
using TeklifAlani.BLL.Services;

namespace TeklifAlani.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ICityService _cityService;
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ICityService cityService, IMapper mapper,IBrandService brandService)
        {
            _httpClient = new HttpClient();
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _cityService = cityService;
            _brandService = brandService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Register()
        {
            ViewData["cities"] = _cityService.GetAll(); 
            ViewData["brands"] = _brandService.GetAll();

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
                model = await UniqueControl(model);

                ModelState.Remove("Logo");
                ModelState.Remove("Username");

                if (!ModelState.IsValid)
                {
                    ViewData["cities"] = _cityService.GetAll();
                    ViewData["brands"] = _brandService.GetAll();
                    return View(model);
                }

                model = await UploadLogoAsync(model, file);

                if (!ModelState.IsValid)
                {
                    ViewData["cities"] = _cityService.GetAll();
                    ViewData["brands"] = _brandService.GetAll();
                    return View(model);
                }

                var appUser = _mapper.Map<ApplicationUser>(model);
                appUser.UserName = model.PhoneNumber;
                // Kullanıcıyı oluştur ve parola belirleyin
                var result = await _userManager.CreateAsync(appUser, model.Password);

                if (result.Succeeded)
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

                // Hataları döndürün
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            ViewData["brands"] = _brandService.GetAll();
            ViewData["cities"] = _cityService.GetAll();
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

        private async Task<CreateApplicationUserDTO> UploadLogoAsync(CreateApplicationUserDTO model, IFormFile file)
        {
            try
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
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Logo Kaydedilemedi!");
            }
            return model;
        }

        private async Task<CreateApplicationUserDTO> UniqueControl(CreateApplicationUserDTO model)
        {
            var userWithSamePhone = await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
            var userWithSameTC = await _userManager.Users.AnyAsync(u => u.TC == model.TC);
            var userWithSameTaxNumber = await _userManager.Users.AnyAsync(u => u.TaxNumber == model.TaxNumber);
            var userWithSameEmail = await _userManager.Users.AnyAsync(u => u.Email == model.Email);

            if (userWithSamePhone || userWithSameTC || userWithSameTaxNumber || userWithSameEmail)
            {
                if (userWithSamePhone)
                {
                    ModelState.AddModelError("PhoneNumber", "Bu telefon numarası zaten kayıtlı.");
                }

                if (userWithSameTC)
                {
                    ModelState.AddModelError("TC", "Bu TC kimlik numarası zaten kayıtlı.");
                }

                if (userWithSameTaxNumber)
                {
                    ModelState.AddModelError("TaxNumber", "Bu vergi numarası zaten kayıtlı.");
                }

                if (userWithSameEmail)
                {
                    ModelState.AddModelError("TaxNumber", "Bu Email adresi zaten kayıtlı.");
                }
            }

            return model;
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
