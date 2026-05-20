using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    // Repository for part data access
    public class PartRepository(AppDbContext context) : IPartRepository
    {
        private readonly AppDbContext _context = context;

        // Get all parts
        public async Task<List<Part>> GetAllAsync()
        {
            return await _context.Parts.AsNoTracking().ToListAsync();
        }

        // Get a single part by id
        public async Task<Part?> GetByIdAsync(int id)
        {
            return await _context.Parts.FirstOrDefaultAsync(part => part.Id == id);
        }

        // Create a new part
        public async Task<Part> AddAsync(Part part)
        {
            await _context.Parts.AddAsync(part);
            await _context.SaveChangesAsync();
            return part;
        }

        // Update an existing part
        public async Task<Part> UpdatePartAsync(Part part)
        {
            _context.Parts.Update(part);
            await _context.SaveChangesAsync();
            return part;
        }

        // Delete a part
        public async Task DeleteAsync(Part part)
        {
            _context.Parts.Remove(part);
            await _context.SaveChangesAsync();
        }
    }
}