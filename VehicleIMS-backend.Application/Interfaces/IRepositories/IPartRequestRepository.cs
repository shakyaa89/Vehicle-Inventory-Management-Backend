using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IRepositories
{
    public interface IPartRequestRepository
    {
        Task<List<PartRequest>> GetAllAsync();
        Task<List<PartRequest>> GetByCustomerIdAsync(long customerId);
        Task<PartRequest?> GetByIdAsync(int id);
        Task<PartRequest> AddAsync(PartRequest partRequest);
        Task<PartRequest> UpdateAsync(PartRequest partRequest);
        Task<bool> CustomerExistsAsync(long customerId);
    }
}
