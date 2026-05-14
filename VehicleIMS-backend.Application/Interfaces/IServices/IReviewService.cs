using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IReviewService
    {
        Task<Review> CreateReviewAsync(ReviewDTO reviewData);
        Task<List<Review>> GetAllReviewsAsync();
        Task<Review?> GetReviewById(int reviewId);
        Task<List<Review>> GetByCustomerId(int customerId);
        Task<Review> UpdateReviewAsync(int reviewId, ReviewUpdateDTO reviewData);
        Task<bool> DeleteReviewAsync(int reviewId);
    }
}
