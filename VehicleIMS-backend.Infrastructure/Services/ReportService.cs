using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Application.Services
{
    // Service to generate financial reports from DB data
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReportService> _logger;

        public ReportService(AppDbContext context, ILogger<ReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Generate financial report for given date range
        public async Task<FinancialReportDTO> GenerateAsync(DateTime from, DateTime to)
        {
            _logger.LogInformation("Generating financial report from {From} to {To}", from, to);
            var sales = await _context.SalesInvoices
                .Where(x => x.CreatedAt >= from && x.CreatedAt <= to)
                .ToListAsync();

            var purchases = await _context.PurchaseInvoices
                .Where(x => x.CreatedAt >= from && x.CreatedAt <= to)
                .ToListAsync();

            var totalSales = sales.Sum(x => x.TotalAmount);
            var totalPurchases = purchases.Sum(x => x.TotalAmount);

            return new FinancialReportDTO
            {
                Title = "Financial Report",
                From = from,
                To = to,

                TotalSales = totalSales,
                TotalPurchases = totalPurchases,
                NetProfit = totalSales - totalPurchases,

                Sales = sales.Select(x => new FinancialReportRowDTO
                {
                    Id = x.Id,
                    Amount = x.TotalAmount,
                    Date = x.CreatedAt,
                    Reference = $"SI-{x.Id}"
                }).ToList(),

                Purchases = purchases.Select(x => new FinancialReportRowDTO
                {
                    Id = x.Id,
                    Amount = x.TotalAmount,
                    Date = x.CreatedAt,
                    Reference = $"PI-{x.Id}"
                }).ToList()
            };
        }

        public async Task<List<RegularCustomerReportDTO>> GetRegularCustomersAsync(DateTime from, DateTime to, int topCount)
        {
            _logger.LogInformation("Generating regular customers report from {From} to {To}", from, to);
            var safeTop = Math.Clamp(topCount, 1, 200);

            var regulars = await _context.SalesInvoices
                .AsNoTracking()
                .Where(x => x.CreatedAt >= from && x.CreatedAt <= to)
                .GroupBy(x => x.CustomerId)
                .Select(group => new
                {
                    CustomerId = group.Key,
                    VisitCount = group.Count(),
                    TotalSpent = group.Sum(x => x.TotalAmount),
                    LastPurchaseAt = group.Max(x => x.CreatedAt)
                })
                .OrderByDescending(x => x.VisitCount)
                .ThenByDescending(x => x.TotalSpent)
                .Take(safeTop)
                .ToListAsync();

            if (regulars.Count == 0)
            {
                return new List<RegularCustomerReportDTO>();
            }

            var usersById = await LoadUsersByIdAsync(regulars.Select(x => x.CustomerId));

            return regulars.Select(item =>
            {
                usersById.TryGetValue(item.CustomerId, out var user);
                return new RegularCustomerReportDTO
                {
                    Id = item.CustomerId,
                    UserName = user?.UserName ?? string.Empty,
                    FullName = user?.FullName ?? string.Empty,
                    Email = user?.Email ?? string.Empty,
                    PhoneNumber = user?.PhoneNumber ?? string.Empty,
                    VisitCount = item.VisitCount,
                    TotalSpent = item.TotalSpent,
                    LastPurchaseAt = item.LastPurchaseAt
                };
            }).ToList();
        }

        public async Task<List<HighSpenderReportDTO>> GetHighSpendersAsync(DateTime from, DateTime to, int topCount)
        {
            _logger.LogInformation("Generating high spender report from {From} to {To}", from, to);
            var safeTop = Math.Clamp(topCount, 1, 200);

            var spenders = await _context.SalesInvoices
                .AsNoTracking()
                .Where(x => x.CreatedAt >= from && x.CreatedAt <= to)
                .GroupBy(x => x.CustomerId)
                .Select(group => new
                {
                    CustomerId = group.Key,
                    VisitCount = group.Count(),
                    TotalSpent = group.Sum(x => x.TotalAmount),
                    LastPurchaseAt = group.Max(x => x.CreatedAt)
                })
                .OrderByDescending(x => x.TotalSpent)
                .ThenByDescending(x => x.VisitCount)
                .Take(safeTop)
                .ToListAsync();

            if (spenders.Count == 0)
            {
                return new List<HighSpenderReportDTO>();
            }

            var usersById = await LoadUsersByIdAsync(spenders.Select(x => x.CustomerId));

            return spenders.Select(item =>
            {
                usersById.TryGetValue(item.CustomerId, out var user);
                return new HighSpenderReportDTO
                {
                    Id = item.CustomerId,
                    UserName = user?.UserName ?? string.Empty,
                    FullName = user?.FullName ?? string.Empty,
                    Email = user?.Email ?? string.Empty,
                    PhoneNumber = user?.PhoneNumber ?? string.Empty,
                    VisitCount = item.VisitCount,
                    TotalSpent = item.TotalSpent,
                    LastPurchaseAt = item.LastPurchaseAt
                };
            }).ToList();
        }

        public async Task<List<PendingCreditReportDTO>> GetPendingCreditsAsync(int olderThanDays)
        {
            var safeDays = Math.Max(0, olderThanDays);
            var cutoff = DateTime.UtcNow.Date.AddDays(-safeDays);

            _logger.LogInformation("Generating pending credits report older than {Days} days", safeDays);

            var pendingCredits = await _context.SalesInvoices
                .AsNoTracking()
                .Where(x => x.IsCredit && x.DueAmount > 0 && x.CreditDueDate != null && x.CreditDueDate <= cutoff)
                .GroupBy(x => x.CustomerId)
                .Select(group => new
                {
                    CustomerId = group.Key,
                    OutstandingAmount = group.Sum(x => x.DueAmount),
                    OldestDueDate = group.Min(x => x.CreditDueDate)
                })
                .OrderByDescending(x => x.OutstandingAmount)
                .ToListAsync();

            if (pendingCredits.Count == 0)
            {
                return new List<PendingCreditReportDTO>();
            }

            var customerIds = pendingCredits.Select(x => x.CustomerId).ToList();
            var customers = await _context.CustomerStats
                .AsNoTracking()
                .Include(customer => customer.User)
                .Where(customer => customerIds.Contains(customer.UserId))
                .ToListAsync();

            var customersById = customers.ToDictionary(customer => customer.UserId);

            return pendingCredits.Select(item =>
            {
                customersById.TryGetValue(item.CustomerId, out var stats);
                var user = stats?.User;

                return new PendingCreditReportDTO
                {
                    Id = item.CustomerId,
                    UserName = user?.UserName ?? string.Empty,
                    FullName = user?.FullName ?? string.Empty,
                    Email = user?.Email ?? string.Empty,
                    PhoneNumber = user?.PhoneNumber ?? string.Empty,
                    CreditBalance = stats?.CreditBalance ?? 0,
                    OutstandingAmount = item.OutstandingAmount,
                    OldestDueDate = item.OldestDueDate
                };
            }).ToList();
        }

        private async Task<Dictionary<long, User>> LoadUsersByIdAsync(IEnumerable<long> userIds)
        {
            var ids = userIds.Distinct().ToList();
            if (ids.Count == 0)
            {
                return new Dictionary<long, User>();
            }

            return await _context.Users
                .AsNoTracking()
                .Where(user => ids.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id);
        }
    }
}