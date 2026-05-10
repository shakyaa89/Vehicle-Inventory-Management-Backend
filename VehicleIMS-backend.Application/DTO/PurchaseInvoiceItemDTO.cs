using System;
using System.ComponentModel.DataAnnotations;

namespace VehicleIMS_backend.Application.DTO
{
    public class PurchaseInvoiceItemDTO
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PartId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PartQuantity { get; set; }

        public decimal SubTotal { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
