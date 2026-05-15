using VehicleIMS_backend.Application.DTO;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IPDFService
    {
        byte[] GenerateFinancialReport(FinancialReportDTO model);
    }
}