using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService, UserManager<User> userManager): ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly UserManager<User> _userManager = userManager;

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer(RegisterDTO registerDTO)
        {
            var customer = await _authService.RegisterCustomer(registerDTO);

            return Ok(customer);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var customer = await _authService.Login(loginDTO);

            return Ok(new
            {
                message = "Login Successful",
                data = customer
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> CheckAuth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

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
    }
}
