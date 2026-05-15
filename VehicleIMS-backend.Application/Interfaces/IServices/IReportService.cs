using System;
using System.Threading.Tasks;
using VehicleIMS_backend.Application.DTO;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IReportService
    {
        Task<FinancialReportDTO> GenerateAsync(DateTime from, DateTime to);
    }
}