using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS_backend.Domain.Models
{
    // Sales invoice header record
    public class SalesInvoice
    {
        [Key]
        public int Id { get; set; }

        // Customer for the invoice
        [Required]
        public long CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public User? Customer { get; set; }

        // Staff who created the invoice
        [Required]
        public long StaffId { get; set; }

        [ForeignKey(nameof(StaffId))]
        public User? Staff { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        // Indicates if loyalty discount was applied
        [Required]
        public bool LoyaltyApplied { get; set; }

        // Indicates if the sale is on credit
        [Required]
        public bool IsCredit { get; set; }

        // Amount remaining for credit sales
        [Required]
        public int DueAmount { get; set; }

        public DateTime? CreditDueDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
