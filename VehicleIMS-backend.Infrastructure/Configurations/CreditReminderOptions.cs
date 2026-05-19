namespace VehicleIMS_backend.Infrastructure.Configurations
{
    public class CreditReminderOptions
    {
        public const string SectionName = "CreditReminder";

        public bool Enabled { get; set; } = true;

        public int IntervalHours { get; set; } = 24;

        public int OlderThanDays { get; set; } = 0;
    }
}
