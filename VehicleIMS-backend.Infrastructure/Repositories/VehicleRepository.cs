using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    // Repository for vehicle data access
    public class VehicleRepository(AppDbContext context) : IVehicleRepository
    {
        private readonly AppDbContext _context = context;

        // Get all vehicles with customer data
        public async Task<List<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles.AsNoTracking().Include(v => v.Customer).ToListAsync();
        }

        // Get a single vehicle by id
        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
        }

        // Get vehicles for a customer
        public async Task<List<Vehicle>> GetByCustomerIdAsync(long customerId)
        {
            return await _context.Vehicles.AsNoTracking().Where(v => v.CustomerId == customerId).ToListAsync();
        }

        // Create a new vehicle
        public async Task<Vehicle> AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        // Delete a vehicle
        public async Task DeleteAsync(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }

        // Check if a customer exists
        public async Task<bool> CustomerExistsAsync(long customerId)
        {
            return await _context.Users.AnyAsync(u => u.Id == customerId);
        }

        // Persist pending changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
