using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleIMS_backend.Application.DTO;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IReportService
    {
        Task<FinancialReportDTO> GenerateAsync(DateTime from, DateTime to);

        Task<List<RegularCustomerReportDTO>> GetRegularCustomersAsync(DateTime from, DateTime to, int topCount);

        Task<List<HighSpenderReportDTO>> GetHighSpendersAsync(DateTime from, DateTime to, int topCount);

        Task<List<PendingCreditReportDTO>> GetPendingCreditsAsync(int olderThanDays);
    }
}