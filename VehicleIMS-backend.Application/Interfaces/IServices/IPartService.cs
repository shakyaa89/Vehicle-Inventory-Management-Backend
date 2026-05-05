using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IPartService
    {
        Task<IEnumerable<Part>> GetAllAsync();
        Task<Part?> GetByIdAsync(int id);
        Task<Part> AddAsync(PartDTO partData);
        Task<Part?> UpdateAsync(int id, PartDTO partData);
        Task<bool> DeleteAsync(int id);
    }
}