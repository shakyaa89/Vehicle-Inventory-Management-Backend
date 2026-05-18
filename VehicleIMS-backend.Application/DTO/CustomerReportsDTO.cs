using System;

namespace VehicleIMS_backend.Application.DTO
{
    public class RegularCustomerReportDTO
    {
        public long Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public int VisitCount { get; set; }

        public decimal TotalSpent { get; set; }

        public DateTime? LastPurchaseAt { get; set; }
    }

    public class HighSpenderReportDTO
    {
        public long Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public int VisitCount { get; set; }

        public decimal TotalSpent { get; set; }

        public DateTime? LastPurchaseAt { get; set; }
    }

    public class PendingCreditReportDTO
    {
        public long Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public int CreditBalance { get; set; }

        public int OutstandingAmount { get; set; }

        public DateTime? OldestDueDate { get; set; }
    }
}
