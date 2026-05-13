using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Infrastructure.Configurations;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class EmailService(IOptions<EmailOptions> options) : IEmailService
    {
        private readonly EmailOptions _options = options.Value;

        public async Task<bool> SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(_options.Host) ||
                string.IsNullOrWhiteSpace(_options.FromEmail) ||
                string.IsNullOrWhiteSpace(toEmail))
            {
                return false;
            }

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _options.FromName,
                    _options.FromEmail));

            message.To.Add(MailboxAddress.Parse(toEmail));

            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            try
            {
                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _options.Host,
                    _options.Port,
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    _options.UserName,
                    _options.Password);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
