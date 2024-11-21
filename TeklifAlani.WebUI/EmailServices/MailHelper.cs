using System.Net.Mail;
using System.Net;

namespace TeklifAlani.WebUI.EmailServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    }

    public class MailHelper : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_configuration["SmtpSettings:Email"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(new MailAddress(to));

            using (var smtp = new SmtpClient())
            {
                smtp.Host = _configuration["SmtpSettings:Host"];
                smtp.Port = int.Parse(_configuration["SmtpSettings:Port"]);
                smtp.EnableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]);
                smtp.Credentials = new NetworkCredential(
                    _configuration["SmtpSettings:Email"],
                    _configuration["SmtpSettings:Password"]
                );

                smtp.Timeout = 10000;

                try
                {
                    await smtp.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email gönderim hatası: {ex.Message}");
                    throw; // Hata fırlatarak işlemi dışarıya bildirin
                }
            }
        }
    }



}
