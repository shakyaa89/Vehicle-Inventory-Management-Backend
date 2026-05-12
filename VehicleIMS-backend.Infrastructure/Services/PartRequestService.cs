using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class PartRequestService(IPartRequestRepository partRequestRepository) : IPartRequestService
    {
        private readonly IPartRequestRepository _partRequestRepository = partRequestRepository;

        public async Task<IEnumerable<PartRequest>> GetAllAsync()
        {
            return await _partRequestRepository.GetAllAsync();
        }

        public async Task<IEnumerable<PartRequest>> GetByCustomerIdAsync(long customerId)
        {
            return await _partRequestRepository.GetByCustomerIdAsync(customerId);
        }

        public async Task<PartRequest?> GetByIdAsync(int id)
        {
            return await _partRequestRepository.GetByIdAsync(id);
        }

        public async Task<PartRequest> AddAsync(PartRequestDTO partRequestData)
        {
            var customerExists = await _partRequestRepository.CustomerExistsAsync(partRequestData.CustomerId);
            if (!customerExists)
                throw new Exception("Customer does not exist.");

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

        public async Task<PartRequest?> CompleteAsync(int id)
        {
            return await UpdateStatusAsync(id, "Completed");
        }

        public async Task<PartRequest?> RejectAsync(int id)
        {
            return await UpdateStatusAsync(id, "Rejected");
        }

        private async Task<PartRequest?> UpdateStatusAsync(int id, string status)
        {
            var existingRequest = await _partRequestRepository.GetByIdAsync(id);

            if (existingRequest is null)
                return null;

            existingRequest.Status = status;
            return await _partRequestRepository.UpdateAsync(existingRequest);
        }
    }
}
