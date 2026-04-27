using System.ComponentModel.DataAnnotations;

namespace VehicleIMS_backend.Domain.Models
{
    public class PartRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PartName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
        [Required]
        public DateTime RequestedDate { get; set; }
    }
}
