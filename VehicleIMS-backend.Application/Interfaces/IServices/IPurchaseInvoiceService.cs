using VehicleIMS_backend.Application.DTO;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IPurchaseInvoiceService
    {
        Task<PurchaseInvoiceDTO?> CreateAsync(PurchaseInvoiceDTO invoiceData, long userId);
        Task<PurchaseInvoiceDTO?> GetByIdAsync(int id);
        Task<List<PurchaseInvoiceDTO>> GetAllAsync();
    }
}
