using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Auth.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IWebHostEnvironment _environment;
        private readonly EmailOptions _options;

        public EmailSender(IOptions<EmailOptions> options, IWebHostEnvironment environment)
        {
            _environment = environment;
            _options = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart("html")
                {
                    Text = htmlMessage
                }
            };

            message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
            message.To.Add(new MailboxAddress(email));

            using var client = new SmtpClient();

            if (_environment.IsDevelopment())
            {
                client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            }

            await client.ConnectAsync(_options.Smtp.Host, _options.Smtp.Port, _options.Smtp.UseSsl);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}
