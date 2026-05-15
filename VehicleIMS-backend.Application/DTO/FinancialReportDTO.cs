using System;
using System.Collections.Generic;

namespace VehicleIMS_backend.Application.DTO
{
    public class FinancialReportDTO
    {
        public string Title { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string VendorName { get; set; } = string.Empty;

        public DateTime From { get; set; }

        public DateTime To { get; set; }


        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal NetProfit { get; set; }

        public List<FinancialReportRowDTO> Sales { get; set; } = new();
        public List<FinancialReportRowDTO> Purchases { get; set; } = new();
    }

    public class FinancialReportRowDTO
    {
        public int Id { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}