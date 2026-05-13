using System.ComponentModel.DataAnnotations;

namespace VehicleIMS_backend.Infrastructure.Configurations
{
    public class EmailOptions
    {
        public const string SectionName = "Email";

        [Required]
        public string Host { get; set; } = string.Empty;

        [Range(1, 65535)]
        public int Port { get; set; } = 587;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        [Required]
        public string FromEmail { get; set; } = string.Empty;

        public string FromName { get; set; } = "VehicleIMS";
    }
}
