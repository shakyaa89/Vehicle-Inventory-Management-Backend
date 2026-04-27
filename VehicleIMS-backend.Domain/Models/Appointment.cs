using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS_backend.Domain.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public User? Customer { get; set; }

        [Required]
        public long StaffId { get; set; }

        [ForeignKey(nameof(StaffId))]
        public User? Staff { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        public int VehicleId { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public Vehicle? Vehicle { get; set; }
    }
}
