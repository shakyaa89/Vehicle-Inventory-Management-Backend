using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    // Repository for sales invoice data access
    public class SalesInvoiceRepository(AppDbContext context) : ISalesInvoiceRepository
    {
        private readonly AppDbContext _context = context;

        // Check if a customer exists
        public async Task<bool> CustomerExistsAsync(long customerId)
        {
            return await _context.CustomerStats.AnyAsync(c => c.UserId == customerId);
        }

        // Get customer stats by user id
        public async Task<CustomerStats?> GetCustomerByUserIdAsync(long customerId)
        {
            return await _context.CustomerStats.FirstOrDefaultAsync(c => c.UserId == customerId);
        }

        // Get parts by their ids
        public async Task<List<Part>> GetPartsByIdsAsync(IEnumerable<int> partIds)
        {
            return await _context.Parts.Where(p => partIds.Contains(p.Id)).ToListAsync();
        }

        // Create a sales invoice and its items in a single transaction
        public async Task CreateAsync(SalesInvoice invoice, List<SalesInvoiceItem> items)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.SalesInvoices.AddAsync(invoice);
            await _context.SalesInvoiceItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        // Get a single sales invoice by id
        public async Task<SalesInvoice?> GetByIdAsync(int id)
        {
            return await _context.SalesInvoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        // Get sales invoice items by invoice id
        public async Task<List<SalesInvoiceItem>> GetItemsByInvoiceIdAsync(int invoiceId)
        {
            return await _context.SalesInvoiceItems.AsNoTracking().Where(i => i.SalesInvoiceId == invoiceId).ToListAsync();
        }

        // Get all sales invoices
        public async Task<List<SalesInvoice>> GetAllAsync()
        {
            return await _context.SalesInvoices.AsNoTracking().OrderByDescending(i => i.CreatedAt).ToListAsync();
        }

        // Get sales invoices for a customer
        public async Task<List<SalesInvoice>> GetByCustomerIdAsync(long customerId)
        {
            return await _context.SalesInvoices.AsNoTracking().Where(i => i.CustomerId == customerId).OrderByDescending(i => i.CreatedAt).ToListAsync();
        }

        // Get a user by id
        public async Task<User?> GetUserByIdAsync(long userId)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == userId);
        }
    }
}
