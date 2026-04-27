using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS_backend.Domain.Models
{
    public class PurchaseInvoiceItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PurchaseInvoiceId { get; set; }

        [ForeignKey(nameof(PurchaseInvoiceId))]
        public PurchaseInvoice? PurchaseInvoice { get; set; }

        [Required]
        public int PartId { get; set; }

        [ForeignKey(nameof(PartId))]
        public Part? Part { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public int PartQuantity { get; set; }

        [Required]
        public decimal SubTotal { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
