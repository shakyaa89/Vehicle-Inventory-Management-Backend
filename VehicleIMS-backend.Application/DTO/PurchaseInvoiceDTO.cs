using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VehicleIMS_backend.Application.DTO
{
    public class PurchaseInvoiceDTO
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int VendorId { get; set; }

        public long UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public List<PurchaseInvoiceItemDTO> Items { get; set; } = new();
    }
}
