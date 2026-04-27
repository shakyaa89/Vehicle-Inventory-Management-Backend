using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IVehicleService
    {
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<Vehicle?> GetByIdAsync(int id);
        Task<Vehicle> AddAsync(VehicleDTO vehicleDTO);
        Task<Vehicle?> UpdateAsync(int id, VehicleDTO vehicleDTO);
        Task<bool> DeleteAsync(int id);
    }
}
