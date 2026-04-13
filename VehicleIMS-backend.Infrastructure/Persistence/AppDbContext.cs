using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Persistence
{
    public class AppDbContext: IdentityDbContext<User, Roles, long>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Roles>().HasData(
                new Roles
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "admin-role"
                },
                new Roles
                {
                    Id = 2,
                    Name = "Staff",
                    NormalizedName = "STAFF",
                    ConcurrencyStamp = "staff-role"
                },
                new Roles
                {
                    Id = 3,
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                    ConcurrencyStamp = "customer-role"
                }
            );

            builder.Ignore<IdentityPasskeyData>();
        }
    }
}
