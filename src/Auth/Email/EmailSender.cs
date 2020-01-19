using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace Auth.Email
{
    public class EmailSender : IEmailSender
    {
        private EmailOptions _options;

        public EmailSender(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MailMessage(_options.From, email, subject, htmlMessage) {IsBodyHtml = true};

            var client = new SmtpClient(_options.Smtp.Host, _options.Smtp.Port) {EnableSsl = _options.Smtp.UseSsl};

            await client.SendMailAsync(message);
        }
    }
}
