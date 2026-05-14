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
    public class AuthController(IAuthService authService, UserManager<User> userManager, ILogger<AuthController> logger): ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<AuthController> _logger = logger;

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer(RegisterDTO registerDTO)
        {
            _logger.LogInformation("Registering customer {UserName}", registerDTO.UserName);
            var customer = await _authService.RegisterCustomer(registerDTO);

            return Ok(customer);
        }

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

        [HttpPost("register/staff")]
        public async Task<IActionResult> RegisterStaff(RegisterDTO registerDTO)
        {
            _logger.LogInformation("Registering staff {UserName}", registerDTO.UserName);
            var staff = await _authService.RegisterStaff(registerDTO);

            return Ok(staff);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> CheckAuth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            _logger.LogInformation("Checking auth for user {UserId}", userId);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token payload" });

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                id = user.Id,
                userName = user.UserName,
                email = user.Email,
                fullName = user.FullName,
                role
            });
        }

        [Authorize]
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers([FromQuery] string? query)
        {
            _logger.LogInformation("Fetching customers with query {Query}", query ?? string.Empty);
            var customers = await _authService.GetCustomersAsync(query);
            return Ok(customers);
        }
    }
}
