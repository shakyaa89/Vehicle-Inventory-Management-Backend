using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VehicleIMS_backend.Application.DTO
{
    public class SalesInvoiceDTO
    {
        public int Id { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public long CustomerId { get; set; }

        public long StaffId { get; set; }

        public decimal TotalAmount { get; set; }

        public bool LoyaltyApplied { get; set; }

        public bool IsCredit { get; set; }

        public int DueAmount { get; set; }

        public DateTime? CreditDueDate { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public List<SalesInvoiceItemDTO> Items { get; set; } = new();
    }
}
