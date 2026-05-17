using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Exceptions;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;
using Microsoft.Extensions.Logging;

namespace VehicleIMS_backend.Infrastructure.Services
{
    // Service handling customer part requests
    public class PartRequestService(IPartRequestRepository partRequestRepository, ILogger<PartRequestService> logger) : IPartRequestService
    {
        private readonly IPartRequestRepository _partRequestRepository = partRequestRepository;
        private readonly ILogger<PartRequestService> _logger = logger;

        // Get all part requests
        public async Task<IEnumerable<PartRequest>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all part requests");
            return await _partRequestRepository.GetAllAsync();
        }

        // Get part requests for a specific customer
        public async Task<IEnumerable<PartRequest>> GetByCustomerIdAsync(long customerId)
        {
            _logger.LogInformation("Fetching part requests for customer {CustomerId}", customerId);
            return await _partRequestRepository.GetByCustomerIdAsync(customerId);
        }

        // Get a single part request by id
        public async Task<PartRequest?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching part request {RequestId}", id);
            return await _partRequestRepository.GetByIdAsync(id) ??
                throw new NotFoundException("Part request not found.");
        }

        // Create a new part request
        public async Task<PartRequest> AddAsync(PartRequestDTO partRequestData)
        {
            _logger.LogInformation("Creating part request for customer {CustomerId}", partRequestData.CustomerId);
            var customerExists = await _partRequestRepository.CustomerExistsAsync(partRequestData.CustomerId);
            if (!customerExists)
                throw new NotFoundException("Customer does not exist.");

            var partRequest = new PartRequest
            {
                CustomerId = partRequestData.CustomerId,
                PartName = partRequestData.PartName,
                Quantity = partRequestData.Quantity,
                Price = 0,
                Status = "Pending",
                RequestedDate = DateTime.UtcNow,
            };

            return await _partRequestRepository.AddAsync(partRequest);
        }

        // Mark a part request as completed
        public async Task<PartRequest?> CompleteAsync(int id)
        {
            _logger.LogInformation("Completing part request {RequestId}", id);
            return await UpdateStatusAsync(id, "Completed");
        }

        // Reject a part request
        public async Task<PartRequest?> RejectAsync(int id)
        {
            _logger.LogInformation("Rejecting part request {RequestId}", id);
            return await UpdateStatusAsync(id, "Rejected");
        }

        // Delete a part request
        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting part request {RequestId}", id);
            var partRequest = await _partRequestRepository.GetByIdAsync(id) ?? throw new NotFoundException("Part Request Not Found!");

            await _partRequestRepository.DeletePartRequestAsync(partRequest);
        }

        // Internal: update the status of a part request
        private async Task<PartRequest?> UpdateStatusAsync(int id, string status)
        {
            var existingRequest = await _partRequestRepository.GetByIdAsync(id);

            if (existingRequest is null)
                throw new NotFoundException("Part request not found.");

            existingRequest.Status = status;
            return await _partRequestRepository.UpdateAsync(existingRequest);
        }
    }
}
