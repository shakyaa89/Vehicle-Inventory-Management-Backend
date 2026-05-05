using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class PartService(IPartRepository partRepository) : IPartService
    {
        private readonly IPartRepository _partRepository = partRepository;

        public async Task<IEnumerable<Part>> GetAllAsync()
        {
            return await _partRepository.GetAllAsync();
        }

        public async Task<Part?> GetByIdAsync(int id)
        {
            return await _partRepository.GetByIdAsync(id);
        }

        public async Task<Part> AddAsync(PartDTO partData)
        {
            var part = new Part
            {
                Name = partData.Name,
                Sku = partData.Sku,
                Price = partData.Price,
                StockQuantity = partData.StockQuantity,
            };

            return await _partRepository.AddAsync(part);
        }

        public async Task<Part?> UpdateAsync(int id, PartDTO partData)
        {
            var existingPart = await _partRepository.GetByIdAsync(id);

            if (existingPart is null)
                return null;

            existingPart.Name = partData.Name;
            existingPart.Sku = partData.Sku;
            existingPart.Price = partData.Price;
            existingPart.StockQuantity = partData.StockQuantity;

            return await _partRepository.UpdatePartAsync(existingPart);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingPart = await _partRepository.GetByIdAsync(id);

            if (existingPart is null)
                return false;

            await _partRepository.DeleteAsync(existingPart);
            return true;
        }
    }
}