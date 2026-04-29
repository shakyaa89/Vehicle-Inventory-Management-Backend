using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IVendorService
    {
        Task<IEnumerable<Vendor>> GetAllAsync();
        Task<Vendor?> GetByIdAsync(int id);
        Task<Vendor> AddAsync(VendorDTO vendorData);
        Task<Vendor?> UpdateAsync(int id, VendorDTO vendorData);
        Task<bool> DeleteAsync(int id);
    }
}
