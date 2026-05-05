using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IRepositories
{
    public interface IPartRepository
    {
        Task<List<Part>> GetAllAsync();
        Task<Part?> GetByIdAsync(int id);
        Task<Part> AddAsync(Part part);
        Task<Part> UpdatePartAsync(Part part);
        Task DeleteAsync(Part part);
    }
}