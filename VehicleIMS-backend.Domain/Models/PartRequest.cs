using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS_backend.Domain.Models
{
    // Customer request for a specific part
    public class PartRequest
    {
        [Key]
        public int Id { get; set; }

        // Related customer
        [Required]
        public long CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public User? Customer { get; set; }

        [Required]
        public string PartName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Request workflow status
        [Required]
        public string Status { get; set; } = string.Empty;
        [Required]
        public DateTime RequestedDate { get; set; }
    }
}
