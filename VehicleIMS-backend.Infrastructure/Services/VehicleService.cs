using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class VehicleService(IVehicleRepository vehicleRepository) : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _vehicleRepository.GetAllAsync();
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _vehicleRepository.GetByIdAsync(id);
        }

        public async Task<Vehicle> AddAsync(VehicleDTO vehicleDTO)
        {
            var customerExists = await _vehicleRepository.CustomerExistsAsync(vehicleDTO.CustomerId);

            if (!customerExists)
                throw new Exception("Customer does not exist.");

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
            var existingVehicle = await _vehicleRepository.GetByIdAsync(id);

            if (existingVehicle is null)
                return null;

            var customerExists = await _vehicleRepository.CustomerExistsAsync(vehicleDTO.CustomerId);

            if (!customerExists)
                throw new Exception("Customer does not exist.");

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
            var existingVehicle = await _vehicleRepository.GetByIdAsync(id);

            if (existingVehicle is null)
                return false;

            await _vehicleRepository.DeleteAsync(existingVehicle);
            return true;
        }
    }
}
