using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS_backend.Domain.Models
{
    public class SalesInvoice
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
        public decimal TotalAmount { get; set; }

        [Required]
        public bool LoyaltyApplied { get; set; }

        [Required]
        public bool IsCredit { get; set; }

        [Required]
        public int DueAmount { get; set; }

        public DateTime? CreditDueDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
