using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IRepositories
{
    public interface IReviewRepository{
        Task<Review> CreateReviewAsync(Review review);
        Task<List<Review>> GetAllReviewsAsync();
        Task<Review?> GetReviewById(int reviewId);
        Task<List<Review>> GetByCustomerIdAsync(int customerId);
        Task<Review> UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Review review);
    }
}