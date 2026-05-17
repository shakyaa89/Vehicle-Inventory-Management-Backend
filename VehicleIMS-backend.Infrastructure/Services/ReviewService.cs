using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using VehicleIMS_backend.Application.Exceptions;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Infrastructure.Services
{
    // Service to create and manage reviews
    public class ReviewService(IReviewRepository repository, ILogger<ReviewService> logger) : IReviewService
    {
        private readonly IReviewRepository _repository = repository;
        private readonly ILogger<ReviewService> _logger = logger;

        // Create a review for an appointment
        public async Task<Review> CreateReviewAsync(ReviewDTO reviewData)
        {
            _logger.LogInformation("Creating review for appointment {AppointmentId}", reviewData.AppointmentId);
            var review = new Review
            {
                CustomerId = reviewData.CustomerId,
                AppointmentId = reviewData.AppointmentId,
                Rating = reviewData.Rating,
                Comment = reviewData.Comment,
            };

            return await _repository.CreateReviewAsync(review);
        }

        // Get all reviews
        public async Task<List<Review>> GetAllReviewsAsync()
        {
            _logger.LogInformation("Fetching all reviews");
            return await _repository.GetAllReviewsAsync();
        }

        // Get a review by id
        public async Task<Review?> GetReviewById(int reviewId)
        {
            _logger.LogInformation("Fetching review {ReviewId}", reviewId);
            return await _repository.GetReviewById(reviewId) ??
                throw new NotFoundException("Review not found.");
        }

        // Get reviews by customer id
        public async Task<List<Review>> GetByCustomerId(int customerId)
        {
            _logger.LogInformation("Fetching reviews for customer {CustomerId}", customerId);
            return await _repository.GetByCustomerIdAsync(customerId);
        }

        // Update an existing review
        public async Task<Review> UpdateReviewAsync(int reviewId, ReviewUpdateDTO reviewData)
        {
            _logger.LogInformation("Updating review {ReviewId}", reviewId);
            var review = await _repository.GetReviewById(reviewId);

            if (review is null)
                throw new NotFoundException("Review not found.");

            review.Rating = reviewData.Rating;
            review.Comment = reviewData.Comment;

            return await _repository.UpdateReviewAsync(review);
        }

        // Delete a review
        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            _logger.LogInformation("Deleting review {ReviewId}", reviewId);
            var review = await _repository.GetReviewById(reviewId);

            if (review is null)
                throw new NotFoundException("Review not found.");

            await _repository.DeleteReviewAsync(review);
            return true;
        }
    }

}
