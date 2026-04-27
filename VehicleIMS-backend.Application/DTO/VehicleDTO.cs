using System.ComponentModel.DataAnnotations;

namespace VehicleIMS_backend.Application.DTO
{
    public class VehicleDTO
    {
        [Range(1, long.MaxValue)]
        public long CustomerId { get; set; }

        [Required]
        public string Make { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Range(1886, 9999)]
        public int Year { get; set; }

        [Required]
        public string VehicleNumber { get; set; } = string.Empty;
    }
}