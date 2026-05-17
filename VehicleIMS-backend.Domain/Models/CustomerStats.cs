using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleIMS_backend.Domain.Models
{
    public class CustomerStats
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public int TotalSpent { get; set; } = 0;

        public int CreditBalance { get; set; } = 0;

    }
}
