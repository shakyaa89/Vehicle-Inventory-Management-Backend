using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    public class VehicleRepository(AppDbContext context) : IVehicleRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vehicle> AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task DeleteAsync(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CustomerExistsAsync(long customerId)
        {
            return await _context.Users.AnyAsync(u => u.Id == customerId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
