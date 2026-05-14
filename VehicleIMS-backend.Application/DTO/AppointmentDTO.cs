using System.ComponentModel.DataAnnotations;

namespace VehicleIMS_backend.Application.DTO
{
    public class AppointmentDTO
    {
        [Range(1, long.MaxValue)]
        public long CustomerId { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int VehicleId { get; set; }
    }

    public class AppointmentResponseDTO
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Range(1, long.MaxValue)]
        public long CustomerId { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int VehicleId { get; set; }

        [Required]
        public string VehicleMake { get; set; } = string.Empty;
    }
}
