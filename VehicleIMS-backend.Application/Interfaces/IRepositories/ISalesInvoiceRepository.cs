using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IRepositories
{
    public interface ISalesInvoiceRepository
    {
        Task<bool> CustomerExistsAsync(long customerId);
        Task<List<Part>> GetPartsByIdsAsync(IEnumerable<int> partIds);
        Task CreateAsync(SalesInvoice invoice, List<SalesInvoiceItem> items);
        Task<SalesInvoice?> GetByIdAsync(int id);
        Task<List<SalesInvoiceItem>> GetItemsByInvoiceIdAsync(int invoiceId);
    }
}
