using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    public class ReviewRepository(AppDbContext context): IReviewRepository{

        private readonly AppDbContext _context = context;

        public async Task<Review> CreateReviewAsync(Review review){
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<List<Review>> GetAllReviewsAsync(){
            return await _context.Reviews.AsNoTracking().ToListAsync();
        }
        public async Task<Review?> GetReviewById(int reviewId){
            return await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
        }
        public async Task<List<Review>> GetByCustomerIdAsync(int customerId){
            return await _context.Reviews.Where(r => r.CustomerId == customerId).ToListAsync();
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task DeleteReviewAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}