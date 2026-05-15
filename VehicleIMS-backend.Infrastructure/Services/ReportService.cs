using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FinancialReportDTO> GenerateAsync(DateTime from, DateTime to)
        {
            var sales = await _context.SalesInvoices
                .Include(x => x.Customer)
                .Where(x => x.CreatedAt >= from && x.CreatedAt <= to)
                .ToListAsync();

            var purchases = await _context.PurchaseInvoices
                .Include(x => x.Vendor)
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
                    Reference = $"Sales-{x.Id}",
                    CustomerName = x.Customer!.FullName,
                }).ToList(),

                Purchases = purchases.Select(x => new FinancialReportRowDTO
                {
                    Id = x.Id,
                    Amount = x.TotalAmount,
                    Date = x.CreatedAt,
                    Reference = $"Purchase-{x.Id}",
                    VendorName = x.Vendor!.Name,
                }).ToList()
            };
        }
    }
}