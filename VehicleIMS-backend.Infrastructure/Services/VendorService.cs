using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class VendorService(IVendorRepository vendorRepository) : IVendorService
    {
        private readonly IVendorRepository _vendorRepository = vendorRepository;

        public async Task<IEnumerable<Vendor>> GetAllAsync()
        {
            return await _vendorRepository.GetAllAsync();
        }

        public async Task<Vendor?> GetByIdAsync(int id)
        {
            return await _vendorRepository.GetByIdAsync(id);
        }

        public async Task<Vendor> AddAsync(VendorDTO vendorData)
        {
            var vendor = new Vendor
            {
                Name = vendorData.Name,
                Contact = vendorData.Contact,
                Address = vendorData.Address,
                Email = vendorData.Email,
            };

            return await _vendorRepository.AddAsync(vendor);
        }

        public async Task<Vendor?> UpdateAsync(int id, VendorDTO vendorData)
        {
            var existingVendor = await _vendorRepository.GetByIdAsync(id);

            if (existingVendor is null)
                return null;

            existingVendor.Name = vendorData.Name;
            existingVendor.Contact = vendorData.Contact;
            existingVendor.Address = vendorData.Address;
            existingVendor.Email = vendorData.Email;

            return await _vendorRepository.UpdateVendorAsync(existingVendor);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingVendor = await _vendorRepository.GetByIdAsync(id);

            if (existingVendor is null)
                return false;

            await _vendorRepository.DeleteAsync(existingVendor);
            return true;
        }
    }
}
