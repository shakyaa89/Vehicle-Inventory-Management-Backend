using VehicleIMS_backend.Application.DTO;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface ISalesInvoiceService
    {
        Task<SalesInvoiceDTO?> CreateAsync(SalesInvoiceDTO invoiceData, long staffId);
        Task<SalesInvoiceDTO?> GetByIdAsync(int id);
    }
}
