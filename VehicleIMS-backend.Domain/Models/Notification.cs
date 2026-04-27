using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS_backend.Domain.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long RecipientId { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public User? Recipient { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

    }
}
