using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Infrastructure.Configurations;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class PendingCreditReminderService(
        IServiceScopeFactory scopeFactory,
        IOptions<CreditReminderOptions> options,
        ILogger<PendingCreditReminderService> logger) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly CreditReminderOptions _options = options.Value;
        private readonly ILogger<PendingCreditReminderService> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("Pending credit reminder service is disabled.");
                return;
            }

            var intervalHours = Math.Max(1, _options.IntervalHours);
            var interval = TimeSpan.FromHours(intervalHours);

            _logger.LogInformation(
                "Pending credit reminder service started. Interval {IntervalHours}h, OlderThanDays {OlderThanDays}",
                intervalHours,
                Math.Max(0, _options.OlderThanDays));

            await RunOnceAsync(stoppingToken);

            using var timer = new PeriodicTimer(interval);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunOnceAsync(stoppingToken);
            }
        }

        private async Task RunOnceAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var olderThanDays = Math.Max(0, _options.OlderThanDays);
                var pendingCredits = await reportService.GetPendingCreditsAsync(olderThanDays);

                if (pendingCredits.Count == 0)
                {
                    _logger.LogInformation("No pending credits found for reminders.");
                    return;
                }

                var attempted = 0;
                var sent = 0;
                var skipped = 0;
                var failed = 0;

                foreach (var item in pendingCredits)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(item.Email))
                    {
                        skipped++;
                        _logger.LogWarning("Skipping reminder for customer {CustomerId} due to missing email.", item.Id);
                        continue;
                    }

                    attempted++;

                    var subject = "Credit payment due reminder";
                    var body = BuildEmailBody(item);

                    try
                    {
                        await emailService.SendEmailAsync(item.Email, subject, body);
                        sent++;
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        _logger.LogError(ex, "Failed to send reminder to customer {CustomerId}", item.Id);
                    }
                }

                _logger.LogInformation(
                    "Pending credit reminder run complete. Attempted {Attempted}, Sent {Sent}, Skipped {Skipped}, Failed {Failed}",
                    attempted,
                    sent,
                    skipped,
                    failed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pending credit reminder run failed.");
            }
        }

        private static string BuildEmailBody(Application.DTO.PendingCreditReportDTO item)
        {
            var customerName = string.IsNullOrWhiteSpace(item.FullName) ? "Customer" : item.FullName;
            var safeName = WebUtility.HtmlEncode(customerName);
            var dueDate = item.OldestDueDate?.ToString("yyyy-MM-dd") ?? "N/A";

            return $"""
                <html>
                <body>
                    <h2>Payment Due Reminder</h2>
                    <p>Dear {safeName},</p>
                    <p>This is a reminder that your credit payment is due.</p>
                    <p><strong>Outstanding amount:</strong> Rs. {item.OutstandingAmount}</p>
                    <p><strong>Due date:</strong> {dueDate}</p>
                    <p>Please visit the service center or contact support to settle the balance.</p>
                </body>
                </html>
                """;
        }
    }
}
