using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Exceptions;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;
using Microsoft.Extensions.Logging;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class VehicleService(IVehicleRepository vehicleRepository, ILogger<VehicleService> logger) : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly ILogger<VehicleService> _logger = logger;

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all vehicles");
            return await _vehicleRepository.GetAllAsync();
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching vehicle {VehicleId}", id);
            return await _vehicleRepository.GetByIdAsync(id) ??
                throw new NotFoundException("Vehicle not found.");
        }

        public async Task<IEnumerable<Vehicle>> GetByCustomerIdAsync(long customerId)
        {
            _logger.LogInformation("Fetching vehicles for customer {CustomerId}", customerId);
            return await _vehicleRepository.GetByCustomerIdAsync(customerId);
        }

        public async Task<Vehicle> AddAsync(VehicleDTO vehicleDTO)
        {
            _logger.LogInformation("Creating vehicle for customer {CustomerId}", vehicleDTO.CustomerId);
            var customerExists = await _vehicleRepository.CustomerExistsAsync(vehicleDTO.CustomerId);

            if (!customerExists)
                throw new NotFoundException("Customer does not exist.");

            var vehicle = new Vehicle
            {
                CustomerId = vehicleDTO.CustomerId,
                Make = vehicleDTO.Make,
                Model = vehicleDTO.Model,
                Year = vehicleDTO.Year,
                VehicleNumber = vehicleDTO.VehicleNumber,
            };

            return await _vehicleRepository.AddAsync(vehicle);
        }

        public async Task<Vehicle?> UpdateAsync(int id, VehicleDTO vehicleDTO)
        {
            _logger.LogInformation("Updating vehicle {VehicleId}", id);
            var existingVehicle = await _vehicleRepository.GetByIdAsync(id);

            if (existingVehicle is null)
                throw new NotFoundException("Vehicle not found.");

            var customerExists = await _vehicleRepository.CustomerExistsAsync(vehicleDTO.CustomerId);

            if (!customerExists)
                throw new NotFoundException("Customer does not exist.");

            existingVehicle.CustomerId = vehicleDTO.CustomerId;
            existingVehicle.Make = vehicleDTO.Make;
            existingVehicle.Model = vehicleDTO.Model;
            existingVehicle.Year = vehicleDTO.Year;
            existingVehicle.VehicleNumber = vehicleDTO.VehicleNumber;

            await _vehicleRepository.SaveChangesAsync();

            return existingVehicle;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting vehicle {VehicleId}", id);
            var existingVehicle = await _vehicleRepository.GetByIdAsync(id);

            if (existingVehicle is null)
                throw new NotFoundException("Vehicle not found.");

            await _vehicleRepository.DeleteAsync(existingVehicle);
            return true;
        }
    }
}
