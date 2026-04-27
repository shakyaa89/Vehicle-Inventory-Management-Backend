using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleIMS_backend.Domain.Models
{
    public class User: IdentityUser<long>
    {
        public string FullName { get; set; } = string.Empty;
    }
}
