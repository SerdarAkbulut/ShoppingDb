using System.Net.Mail;
using System.Net;

namespace ShoppingApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
    public class EmailService: IEmailService
    {

        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com") // örneğin smtp.gmail.com
            {
                Port = 587,
                Credentials = new NetworkCredential(_configuration["EmailService:Email"], _configuration["EmailService:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage(_configuration["EmailService:Email"], toEmail, subject, message);
            mailMessage.IsBodyHtml = true;

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
