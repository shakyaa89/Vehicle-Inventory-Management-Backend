using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleIMS_backend.Domain.Models
{
    public class CustomerStats
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long UserId { get; set; }

        public User? User { get; set; }

        public int TotalSpent { get; set; } = 0;

    }
}
