using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    // Controller for review endpoints
    public class ReviewController(IReviewService reviewService, ILogger<ReviewController> logger): ControllerBase
    {
        private readonly IReviewService _reviewService = reviewService;
        private readonly ILogger<ReviewController> _logger = logger;

        // Create a new review
        [HttpPost("create")]
        public async Task<IActionResult> CreateReviewAsync(ReviewDTO reviewData)
        {
            _logger.LogInformation("Creating review for appointment {AppointmentId}", reviewData.AppointmentId);
            return Ok(await _reviewService.CreateReviewAsync(reviewData));
        }

        // Get reviews for a customer
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetReviewsByCustomerId(int customerId)
        {
            _logger.LogInformation("Fetching reviews for customer {CustomerId}", customerId);
            return Ok(await _reviewService.GetByCustomerId(customerId));
        }

        // Get all reviews
        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            _logger.LogInformation("Fetching all reviews");
            return Ok(await _reviewService.GetAllReviewsAsync());
        }

        // Get a single review by id
        [HttpGet("{reviewId:int}")]
        public async Task<IActionResult> GetReviewById(int reviewId)
        {
            _logger.LogInformation("Fetching review {ReviewId}", reviewId);
            return Ok(await _reviewService.GetReviewById(reviewId));
        }

        // Update an existing review
        [HttpPut("{reviewId:int}")]
        public async Task<IActionResult> UpdateReview(int reviewId, ReviewUpdateDTO reviewData)
        {
            _logger.LogInformation("Updating review {ReviewId}", reviewId);
            var review = await _reviewService.UpdateReviewAsync(reviewId, reviewData);
            return Ok(review);
        }

        // Delete a review
        [HttpDelete("{reviewId:int}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            _logger.LogInformation("Deleting review {ReviewId}", reviewId);
            await _reviewService.DeleteReviewAsync(reviewId);
            return Ok(new { message = "Review deleted successfully" });
        }
    }
}
