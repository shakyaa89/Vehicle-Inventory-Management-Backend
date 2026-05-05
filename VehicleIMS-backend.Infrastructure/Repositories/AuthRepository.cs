using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    public class AuthRepository(AppDbContext context) : IAuthRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateCustomerAsync(CustomerStats customer)
        {
            await _context.CustomerStats.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CustomerStats>> GetCustomersWithUsersAsync(string? query)
        {
            var customersQuery = _context.CustomerStats
                .Include(c => c.User)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.ToLower();

                customersQuery = customersQuery.Where(c =>
                    (c.User != null) &&
                    (
                        c.User.FullName.ToLower().Contains(query) ||
                        c.User.UserName.ToLower().Contains(query) ||
                        c.User.PhoneNumber.ToLower().Contains(query)
                    )
                );
            }

            return await customersQuery.ToListAsync();
        }
    }
}
