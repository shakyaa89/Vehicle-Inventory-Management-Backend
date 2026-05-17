using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Exceptions;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;
using Microsoft.Extensions.Logging;

namespace VehicleIMS_backend.Infrastructure.Services
{
    // Service for managing parts inventory
    public class PartService(IPartRepository partRepository, ILogger<PartService> logger) : IPartService
    {
        private readonly IPartRepository _partRepository = partRepository;
        private readonly ILogger<PartService> _logger = logger;

        // Get all parts
        public async Task<IEnumerable<Part>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all parts");
            return await _partRepository.GetAllAsync();
        }

        // Get a part by id
        public async Task<Part?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching part {PartId}", id);
            return await _partRepository.GetByIdAsync(id) ??
                throw new NotFoundException("Part not found.");
        }

        // Add a new part to inventory
        public async Task<Part> AddAsync(PartDTO partData)
        {
            _logger.LogInformation("Creating part {PartName} with SKU {Sku}", partData.Name, partData.Sku);
            var part = new Part
            {
                Name = partData.Name,
                Sku = partData.Sku,
                Price = partData.Price,
                StockQuantity = partData.StockQuantity,
            };

            return await _partRepository.AddAsync(part);
        }

        // Update existing part details
        public async Task<Part?> UpdateAsync(int id, PartDTO partData)
        {
            _logger.LogInformation("Updating part {PartId}", id);
            var existingPart = await _partRepository.GetByIdAsync(id);

            if (existingPart is null)
                throw new NotFoundException("Part not found.");

            existingPart.Name = partData.Name;
            existingPart.Sku = partData.Sku;
            existingPart.Price = partData.Price;
            existingPart.StockQuantity = partData.StockQuantity;

            return await _partRepository.UpdatePartAsync(existingPart);
        }

        // Delete a part
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting part {PartId}", id);
            var existingPart = await _partRepository.GetByIdAsync(id);

            if (existingPart is null)
                throw new NotFoundException("Part not found.");

            await _partRepository.DeleteAsync(existingPart);
            return true;
        }
    }
}