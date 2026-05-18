using System;
using System.Collections.Generic;
using VehicleIMS_backend.Application.DTO;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IPDFService
    {
        byte[] GenerateFinancialReport(FinancialReportDTO model);

        byte[] GenerateRegularCustomersReport(List<RegularCustomerReportDTO> customers, DateTime from, DateTime to);

        byte[] GenerateHighSpendersReport(List<HighSpenderReportDTO> customers, DateTime from, DateTime to);

        byte[] GeneratePendingCreditsReport(List<PendingCreditReportDTO> customers, int olderThanDays);
    }
}