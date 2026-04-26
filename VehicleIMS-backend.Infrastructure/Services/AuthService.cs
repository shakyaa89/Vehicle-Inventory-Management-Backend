using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class AuthService(UserManager<User> userManager, RoleManager<Roles> roleManager, IAuthRepository authRepository, SignInManager<User> signInManager): IAuthService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<Roles> _roleManager = roleManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IAuthRepository _authRepository = authRepository;

        public async Task<Customer> RegisterCustomer(RegisterDTO registerDTO)
        {
            var user = new User
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                FullName = registerDTO.FullName,
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new Roles { Name = "Customer" });
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            // Create customer in DB
            var customer = new Customer
            {
                UserId = user.Id,
                TotalSpent = 0,
            };

            await _authRepository.CreateCustomerAsync(customer);

            return customer;
        }

        public async Task<User> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.UserName) ?? throw new UnauthorizedAccessException("Invalid username or password");

            var result = await _signInManager.PasswordSignInAsync(loginDTO.UserName, loginDTO.Password, false, true);

            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid username or password");

            return user;
        }
    }
}
