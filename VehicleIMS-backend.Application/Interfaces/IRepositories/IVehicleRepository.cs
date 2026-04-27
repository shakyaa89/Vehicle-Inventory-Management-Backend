using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IRepositories
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetAllAsync();
        Task<Vehicle?> GetByIdAsync(int id);
        Task<Vehicle> AddAsync(Vehicle vehicle);
        Task DeleteAsync(Vehicle vehicle);
        Task<bool> CustomerExistsAsync(long customerId);
        Task SaveChangesAsync();
    }
}
