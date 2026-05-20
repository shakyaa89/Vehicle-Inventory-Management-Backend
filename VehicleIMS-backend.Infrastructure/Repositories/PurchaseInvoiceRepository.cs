using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    // Repository for purchase invoice data access
    public class PurchaseInvoiceRepository(AppDbContext context) : IPurchaseInvoiceRepository
    {
        private readonly AppDbContext _context = context;

        // Check if a vendor exists
        public async Task<bool> VendorExistsAsync(int vendorId)
        {
            return await _context.Vendors.AnyAsync(v => v.Id == vendorId);
        }

        // Get parts by their ids
        public async Task<List<Part>> GetPartsByIdsAsync(IEnumerable<int> partIds)
        {
            return await _context.Parts.Where(p => partIds.Contains(p.Id)).ToListAsync();
        }

        // Create a purchase invoice and its items in a single transaction
        public async Task CreateAsync(PurchaseInvoice invoice, List<PurchaseInvoiceItem> items)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.PurchaseInvoices.AddAsync(invoice);
            await _context.PurchaseInvoiceItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        // Get a single purchase invoice by id
        public async Task<PurchaseInvoice?> GetByIdAsync(int id)
        {
            return await _context.PurchaseInvoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        // Get purchase invoice items by invoice id
        public async Task<List<PurchaseInvoiceItem>> GetItemsByInvoiceIdAsync(int invoiceId)
        {
            return await _context.PurchaseInvoiceItems.AsNoTracking().Where(i => i.PurchaseInvoiceId == invoiceId).ToListAsync();
        }

        // Get all purchase invoices
        public async Task<List<PurchaseInvoice>> GetAllAsync()
        {
            return await _context.PurchaseInvoices.AsNoTracking().OrderByDescending(i => i.CreatedAt).ToListAsync();
        }
    }
}
