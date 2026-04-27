using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace VehicleIMS_backend.Infrastructure.Configurations
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        [Required]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        public string Audience { get; set; } = string.Empty;

        [Required]
        [MinLength(32)]
        public string Secret { get; set; } = string.Empty;

        public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));

        [Range(1, int.MaxValue)]
        public int ExpiryHours { get; init; }

        public DateTime ExpiryDate => DateTime.UtcNow.AddHours(ExpiryHours);

    }
}
