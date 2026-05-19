using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Exceptions;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Services
{
    // Authentication and user management service
    public class AuthService(UserManager<User> userManager, RoleManager<Roles> roleManager, IAuthRepository authRepository, SignInManager<User> signInManager, IJwtTokenService jwtTokenService, ILogger<AuthService> logger) : IAuthService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<Roles> _roleManager = roleManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly ILogger<AuthService> _logger = logger;

        // Register a new customer account
        public async Task<CustomerStats> RegisterCustomer(RegisterDTO registerDTO)
        {
            _logger.LogInformation("Registering customer {UserName}", registerDTO.UserName);
            var user = new User
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                FullName = registerDTO.FullName,
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new Roles { Name = "Customer" });
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            // Create customer in DB
            var customer = new CustomerStats
            {
                UserId = user.Id,
                TotalSpent = 0,
                CreditBalance = 0,
            };

            await _authRepository.CreateCustomerAsync(customer);

            _logger.LogInformation("Customer registered with user id {UserId}", user.Id);

            return customer;
        }

        // Register a new staff account
        public async Task<User> RegisterStaff(RegisterDTO registerDTO)
        {
            _logger.LogInformation("Registering staff {UserName}", registerDTO.UserName);
            var user = new User
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                FullName = registerDTO.FullName,
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync("Staff"))
            {
                await _roleManager.CreateAsync(new Roles { Name = "Staff" });
            }

            await _userManager.AddToRoleAsync(user, "Staff");

            _logger.LogInformation("Staff registered with user id {UserId}", user.Id);

            return user;
        }

        // Authenticate user and return JWT token + user info
        public async Task<object> Login(LoginDTO loginDTO)
        {
            _logger.LogInformation("User login attempt for {UserName}", loginDTO.UserName);
            var user = await _userManager.FindByNameAsync(loginDTO.UserName) ?? throw new UnauthorizedAccessException("Invalid username or password");

            var result = await _signInManager.PasswordSignInAsync(loginDTO.UserName, loginDTO.Password, false, true);

            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid username or password");


            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            var token = _jwtTokenService.GenerateUserToken(user);

            _logger.LogInformation("User login succeeded for {UserId}", user.Id);
            
            return new
            {
                jwtToken = token,
                user = new
                {
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    fullName = user.FullName,
                    phoneNumber = user.PhoneNumber,
                    role
                }
            };
        }

        // Search customers with optional query
        public async Task<IEnumerable<CustomerStatsDTO>> GetCustomersAsync(string? query)
        {
            _logger.LogInformation("Fetching customers with query {Query}", query ?? string.Empty);
            var customers = await _authRepository.GetCustomersWithUsersAsync(query);

            return customers
                .Select(customer => new CustomerStatsDTO
                {
                    Id = customer.User?.Id ?? 0,
                    UserName = customer.User?.UserName ?? string.Empty,
                    FullName = customer.User?.FullName ?? string.Empty,
                    Email = customer.User?.Email ?? string.Empty,
                    PhoneNumber = customer.User?.PhoneNumber ?? string.Empty,
                    CreditBalance = customer.CreditBalance
                })
                .Where(customer => customer.Id != 0)
                .OrderBy(customer => customer.FullName)
                .ToList();
        }

        public async Task<object> UpdateProfileAsync(long userId, UpdateProfileDTO updateProfileDTO)
        {
            _logger.LogInformation("Updating profile for user {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new NotFoundException("User not found");

            user.UserName = updateProfileDTO.UserName.Trim();
            user.FullName = updateProfileDTO.FullName.Trim();
            user.Email = updateProfileDTO.Email.Trim();
            user.PhoneNumber = updateProfileDTO.PhoneNumber.Trim();

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            return new
            {
                id = user.Id,
                userName = user.UserName,
                email = user.Email,
                fullName = user.FullName,
                phoneNumber = user.PhoneNumber,
                role
            };
        }

        public async Task ChangePasswordAsync(long userId, ChangePasswordDTO changePasswordDTO)
        {
            _logger.LogInformation("Changing password for user {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new NotFoundException("User not found");

            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordDTO.CurrentPassword,
                changePasswordDTO.NewPassword
            );

            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<IEnumerable<StaffInfoDTO>> GetStaffAsync(string? query)
        {
            _logger.LogInformation("Fetching staff with query {Query}", query ?? string.Empty);

            var staffMembers = await _userManager.GetUsersInRoleAsync("Staff");
            var staffQuery = staffMembers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalized = query.Trim().ToLower();
                staffQuery = staffQuery.Where(staff =>
                    (staff.FullName ?? string.Empty).ToLower().Contains(normalized) ||
                    (staff.UserName ?? string.Empty).ToLower().Contains(normalized) ||
                    (staff.Email ?? string.Empty).ToLower().Contains(normalized) ||
                    (staff.PhoneNumber ?? string.Empty).ToLower().Contains(normalized)
                );
            }

            return staffQuery
                .OrderBy(staff => staff.FullName)
                .Select(staff => new StaffInfoDTO
                {
                    Id = staff.Id,
                    UserName = staff.UserName ?? string.Empty,
                    FullName = staff.FullName ?? string.Empty,
                    Email = staff.Email ?? string.Empty,
                    PhoneNumber = staff.PhoneNumber ?? string.Empty
                })
                .ToList();
        }
    }
}
