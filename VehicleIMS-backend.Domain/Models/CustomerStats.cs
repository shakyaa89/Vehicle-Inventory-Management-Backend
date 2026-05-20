using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleIMS_backend.Domain.Models
{
    // Aggregated customer spending metrics
    public class CustomerStats
    {
        [Key]
        public int Id { get; set; }

        // Related user
        [Required]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        // Total spend across invoices
        public int TotalSpent { get; set; } = 0;

        // Outstanding credit balance
        public int CreditBalance { get; set; } = 0;

    }
}
