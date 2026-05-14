using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Exceptions;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;
using Microsoft.Extensions.Logging;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class VendorService(IVendorRepository vendorRepository, ILogger<VendorService> logger) : IVendorService
    {
        private readonly IVendorRepository _vendorRepository = vendorRepository;
        private readonly ILogger<VendorService> _logger = logger;

        public async Task<IEnumerable<Vendor>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all vendors");
            return await _vendorRepository.GetAllAsync();
        }

        public async Task<Vendor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching vendor {VendorId}", id);
            return await _vendorRepository.GetByIdAsync(id) ??
                throw new NotFoundException("Vendor not found.");
        }

        public async Task<Vendor> AddAsync(VendorDTO vendorData)
        {
            _logger.LogInformation("Creating vendor {VendorName}", vendorData.Name);
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
            _logger.LogInformation("Updating vendor {VendorId}", id);
            var existingVendor = await _vendorRepository.GetByIdAsync(id);

            if (existingVendor is null)
                throw new NotFoundException("Vendor not found.");

            existingVendor.Name = vendorData.Name;
            existingVendor.Contact = vendorData.Contact;
            existingVendor.Address = vendorData.Address;
            existingVendor.Email = vendorData.Email;

            return await _vendorRepository.UpdateVendorAsync(existingVendor);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting vendor {VendorId}", id);
            var existingVendor = await _vendorRepository.GetByIdAsync(id);

            if (existingVendor is null)
                throw new NotFoundException("Vendor not found.");

            await _vendorRepository.DeleteAsync(existingVendor);
            return true;
        }
    }
}
