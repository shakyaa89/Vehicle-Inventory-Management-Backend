using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IPartRequestService
    {
        Task<IEnumerable<PartRequest>> GetAllAsync();
        Task<IEnumerable<PartRequest>> GetByCustomerIdAsync(long customerId);
        Task<PartRequest?> GetByIdAsync(int id);
        Task<PartRequest> AddAsync(PartRequestDTO partRequestData);
        Task<PartRequest?> CompleteAsync(int id);
        Task<PartRequest?> RejectAsync(int id);

        Task DeleteAsync(int id);
    }
}
