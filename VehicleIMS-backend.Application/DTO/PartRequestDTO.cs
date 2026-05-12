using System.ComponentModel.DataAnnotations;

namespace VehicleIMS_backend.Application.DTO
{
    public class PartRequestDTO
    {
        [Range(1, long.MaxValue)]
        public long CustomerId { get; set; }

        [Required]
        public string PartName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
