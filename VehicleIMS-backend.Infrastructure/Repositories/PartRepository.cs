using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    public class PartRepository(AppDbContext context) : IPartRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Part>> GetAllAsync()
        {
            return await _context.Parts.AsNoTracking().ToListAsync();
        }

        public async Task<Part?> GetByIdAsync(int id)
        {
            return await _context.Parts.FirstOrDefaultAsync(part => part.Id == id);
        }

        public async Task<Part> AddAsync(Part part)
        {
            await _context.Parts.AddAsync(part);
            await _context.SaveChangesAsync();
            return part;
        }

        public async Task<Part> UpdatePartAsync(Part part)
        {
            _context.Parts.Update(part);
            await _context.SaveChangesAsync();
            return part;
        }

        public async Task DeleteAsync(Part part)
        {
            _context.Parts.Remove(part);
            await _context.SaveChangesAsync();
        }
    }
}