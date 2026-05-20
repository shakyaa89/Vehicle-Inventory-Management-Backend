using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleIMS_backend.Domain.Models
{
    // Application user profile
    public class User: IdentityUser<long>
    {
        // Display name for the user
        public string FullName { get; set; } = string.Empty;
    }
}
