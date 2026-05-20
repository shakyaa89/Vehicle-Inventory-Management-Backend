using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    // Repository for vendor data access
    public class VendorRepository(AppDbContext context) : IVendorRepository
    {
        private readonly AppDbContext _context = context;

        // Get all vendors
        public async Task<List<Vendor>> GetAllAsync()
        {
            return await _context.Vendors.AsNoTracking().ToListAsync();
        }

        // Get a single vendor by id
        public async Task<Vendor?> GetByIdAsync(int id)
        {
            return await _context.Vendors.FirstOrDefaultAsync(v => v.Id == id);
        }

        // Create a new vendor
        public async Task<Vendor> AddAsync(Vendor vendor)
        {
            await _context.Vendors.AddAsync(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }

        // Update an existing vendor
        public async Task<Vendor> UpdateVendorAsync(Vendor vendor)
        {
            _context.Vendors.Update(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }

        // Delete a vendor
        public async Task DeleteAsync(Vendor vendor)
        {
            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
        }

    }
}
