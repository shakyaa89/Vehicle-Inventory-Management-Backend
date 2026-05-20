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
    [Route("api/auth")]
    [ApiController]
    // Controller for authentication and profile endpoints
    public class AuthController(IAuthService authService, UserManager<User> userManager, ILogger<AuthController> logger): ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<AuthController> _logger = logger;

        // Register a customer account
        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer(RegisterDTO registerDTO)
        {
            _logger.LogInformation("Registering customer {UserName}", registerDTO.UserName);
            var customer = await _authService.RegisterCustomer(registerDTO);

            return Ok(customer);
        }

        // Authenticate a user and return auth data
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var customer = await _authService.Login(loginDTO);

            _logger.LogInformation("User login succeeded for {UserName}", loginDTO.UserName);

            return Ok(new
            {
                message = "Login Successful",
                data = customer
            });
        }

        // Register a staff account
        [HttpPost("register/staff")]
        public async Task<IActionResult> RegisterStaff(RegisterDTO registerDTO)
        {
            _logger.LogInformation("Registering staff {UserName}", registerDTO.UserName);
            var staff = await _authService.RegisterStaff(registerDTO);

            return Ok(staff);
        }

        // Get the current authenticated user
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> CheckAuth()
        {
            // Read claims from the token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            _logger.LogInformation("Checking auth for user {UserId}", userId);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token payload" });

            // Load the user profile
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                id = user.Id,
                userName = user.UserName,
                email = user.Email,
                fullName = user.FullName,
                phoneNumber = user.PhoneNumber,
                role
            });
        }

        // Update the authenticated user's profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO updateProfileDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token payload" });

            if (!long.TryParse(userId, out var parsedUserId))
                return Unauthorized(new { message = "Invalid user id" });

            var updatedUser = await _authService.UpdateProfileAsync(parsedUserId, updateProfileDTO);

            return Ok(new
            {
                message = "Profile updated successfully",
                data = updatedUser
            });
        }

        // Change the authenticated user's password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token payload" });

            if (!long.TryParse(userId, out var parsedUserId))
                return Unauthorized(new { message = "Invalid user id" });

            await _authService.ChangePasswordAsync(parsedUserId, changePasswordDTO);

            return Ok(new
            {
                message = "Password updated successfully"
            });
        }

        // Get customers with optional search query
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers([FromQuery] string? query)
        {
            _logger.LogInformation("Fetching customers with query {Query}", query ?? string.Empty);
            var customers = await _authService.GetCustomersAsync(query);
            return Ok(customers);
        }

        // Get staff users (admin only)
        [HttpGet("staff")]
        public async Task<IActionResult> GetStaff([FromQuery] string? query)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            // Restrict staff list to admins
            if (role != "Admin")
                return Forbid();

            _logger.LogInformation("Fetching staff with query {Query}", query ?? string.Empty);
            var staff = await _authService.GetStaffAsync(query);
            return Ok(staff);
        }
    }
}
