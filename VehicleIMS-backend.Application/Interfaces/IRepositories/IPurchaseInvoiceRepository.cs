using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IRepositories
{
    public interface IPurchaseInvoiceRepository
    {
        Task<bool> VendorExistsAsync(int vendorId);
        Task<List<Part>> GetPartsByIdsAsync(IEnumerable<int> partIds);
        Task CreateAsync(PurchaseInvoice invoice, List<PurchaseInvoiceItem> items);
        Task<PurchaseInvoice?> GetByIdAsync(int id);
        Task<List<PurchaseInvoiceItem>> GetItemsByInvoiceIdAsync(int invoiceId);
        Task<List<PurchaseInvoice>> GetAllAsync();
    }
}
