using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using VehicleIMS_backend.Application.Exceptions;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Infrastructure.Configurations;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger) : IEmailService
    {
        private readonly EmailOptions _options = options.Value;
        private readonly ILogger<EmailService> _logger = logger;

        public async Task<bool> SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(_options.Host) ||
                string.IsNullOrWhiteSpace(_options.FromEmail))
            {
                throw new Exception("Email configuration is missing.");
            }

            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new BadRequestException("Recipient email is required.");
            }

            _logger.LogInformation("Sending email to {ToEmail} with subject {Subject}", toEmail, subject);

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

                _logger.LogInformation("Email sent to {ToEmail}", toEmail);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
                throw new Exception("Failed to send email.");
            }
        }
    }
}
