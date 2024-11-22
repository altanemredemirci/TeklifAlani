using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeklifAlani.BLL.Abstract;
using TeklifAlani.BLL.Services;
using TeklifAlani.Core.Identity;
using TeklifAlani.DAL.Abstract;
using TeklifAlani.DAL.Concrete;
using TeklifAlani.DAL.Context;
using TeklifAlani.WebUI.EmailServices;
using TeklifAlani.WebUI.Models;
using TeklifAlani.WEBUI.Context;

namespace TeklifAlani.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IEmailSender, MailHelper>();

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductDal, EfCoreProductDal>();


            // appsettings.json dosyasýndan baðlantý dizesini alýyoruz
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // ApplicationIdentityDbContext için yapýlandýrma
            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(connectionString));

            // DataContext için yapýlandýrma
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
              .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.LogoutPath = "/Account/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.SlidingExpiration = true;
                    options.Cookie = new CookieBuilder()
                    {
                        HttpOnly = true,
                        Name = "TeklifAlani.Security.Cookie",
                        SameSite = SameSiteMode.Strict
                    };

                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            if (context.Request.IsAjaxRequest())
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            }
                            else
                            {
                                context.Response.Redirect(context.RedirectUri);
                            }
                            return Task.CompletedTask;
                        },
                        OnRedirectToAccessDenied = context =>
                        {
                            if (context.Request.IsAjaxRequest())
                            {
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            }
                            else
                            {
                                context.Response.Redirect(context.RedirectUri);
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
