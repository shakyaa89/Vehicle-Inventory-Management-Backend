using System;

namespace VehicleIMS_backend.Application.DTO
{
    public class CustomerStatsDTO
    {
        public long Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
