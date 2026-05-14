using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    public class PartRequestRepository(AppDbContext context) : IPartRequestRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<PartRequest>> GetAllAsync()
        {
            return await _context.PartRequests.AsNoTracking().ToListAsync();
        }

        public async Task<List<PartRequest>> GetByCustomerIdAsync(long customerId)
        {
            return await _context.PartRequests.Where(request => request.CustomerId == customerId).AsNoTracking().ToListAsync();
        }

        public async Task<PartRequest?> GetByIdAsync(int id)
        {
            return await _context.PartRequests.AsNoTracking().FirstOrDefaultAsync(request => request.Id == id);
        }

        public async Task<PartRequest> AddAsync(PartRequest partRequest)
        {
            await _context.PartRequests.AddAsync(partRequest);
            await _context.SaveChangesAsync();
            return partRequest;
        }

        public async Task<PartRequest> UpdateAsync(PartRequest partRequest)
        {
            _context.PartRequests.Update(partRequest);
            await _context.SaveChangesAsync();
            return partRequest;
        }

        public async Task DeletePartRequestAsync(PartRequest partRequest)
        {
            _context.Remove(partRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CustomerExistsAsync(long customerId)
        {
            return await _context.Users.AnyAsync(user => user.Id == customerId);
        }
    }
}
