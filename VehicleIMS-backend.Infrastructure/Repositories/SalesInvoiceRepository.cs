using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    public class SalesInvoiceRepository(AppDbContext context) : ISalesInvoiceRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<bool> CustomerExistsAsync(long customerId)
        {
            return await _context.CustomerStats.AnyAsync(c => c.UserId == customerId);
        }

        public async Task<List<Part>> GetPartsByIdsAsync(IEnumerable<int> partIds)
        {
            return await _context.Parts
                .Where(p => partIds.Contains(p.Id))
                .ToListAsync();
        }

        public async Task CreateAsync(SalesInvoice invoice, List<SalesInvoiceItem> items)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.SalesInvoices.AddAsync(invoice);
            await _context.SalesInvoiceItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        public async Task<SalesInvoice?> GetByIdAsync(int id)
        {
            return await _context.SalesInvoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<SalesInvoiceItem>> GetItemsByInvoiceIdAsync(int invoiceId)
        {
            return await _context.SalesInvoiceItems
                .AsNoTracking()
                .Where(i => i.SalesInvoiceId == invoiceId)
                .ToListAsync();
        }
    }
}
