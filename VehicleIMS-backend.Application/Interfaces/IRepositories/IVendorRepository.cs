using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IRepositories
{
    public interface IVendorRepository
    {
        Task<List<Vendor>> GetAllAsync();
        Task<Vendor?> GetByIdAsync(int id);
        Task<Vendor> AddAsync(Vendor vendor);
        Task<Vendor> UpdateVendorAsync(Vendor vendor);
        Task DeleteAsync(Vendor vendor);
    }
}
