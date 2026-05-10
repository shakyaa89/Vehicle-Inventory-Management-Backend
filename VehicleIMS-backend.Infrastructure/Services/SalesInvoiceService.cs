using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class SalesInvoiceService(ISalesInvoiceRepository salesInvoiceRepository) : ISalesInvoiceService
    {
        private readonly ISalesInvoiceRepository _salesInvoiceRepository = salesInvoiceRepository;

        public async Task<SalesInvoiceDTO?> CreateAsync(SalesInvoiceDTO invoiceData, long staffId)
        {
            if (invoiceData.Items is null || invoiceData.Items.Count == 0)
                return null;

            var customerExists = await _salesInvoiceRepository.CustomerExistsAsync(invoiceData.CustomerId);
            if (!customerExists)
                return null;

            var partIds = invoiceData.Items.Select(i => i.PartId).Distinct().ToList();
            var parts = await _salesInvoiceRepository.GetPartsByIdsAsync(partIds);

            if (parts.Count != partIds.Count)
                return null;

            var partsById = parts.ToDictionary(part => part.Id);
            var now = DateTime.UtcNow;
            var items = new List<SalesInvoiceItem>();
            decimal totalAmount = 0;

            foreach (var item in invoiceData.Items)
            {
                if (item.PartQuantity <= 0 || item.UnitPrice < 0)
                    return null;

                if (!partsById.TryGetValue(item.PartId, out var part))
                    return null;

                if (part.StockQuantity < item.PartQuantity)
                    return null;

                part.StockQuantity -= item.PartQuantity;

                var subTotal = item.UnitPrice * item.PartQuantity;
                totalAmount += subTotal;

                items.Add(new SalesInvoiceItem
                {
                    PartId = item.PartId,
                    UnitPrice = item.UnitPrice,
                    PartQuantity = item.PartQuantity,
                    SubTotal = subTotal,
                    CreatedAt = now
                });
            }

            var invoice = new SalesInvoice
            {
                CustomerId = invoiceData.CustomerId,
                StaffId = staffId,
                TotalAmount = totalAmount,
                LoyaltyApplied = invoiceData.LoyaltyApplied,
                IsCredit = invoiceData.IsCredit,
                DueAmount = invoiceData.IsCredit ? Convert.ToInt32(Math.Round(totalAmount)) : 0,
                CreditDueDate = invoiceData.IsCredit ? invoiceData.CreditDueDate : null,
                CreatedAt = now
            };

            foreach (var item in items)
            {
                item.SalesInvoice = invoice;
            }

            await _salesInvoiceRepository.CreateAsync(invoice, items);

            return new SalesInvoiceDTO
            {
                Id = invoice.Id,
                CustomerId = invoice.CustomerId,
                StaffId = invoice.StaffId,
                TotalAmount = invoice.TotalAmount,
                LoyaltyApplied = invoice.LoyaltyApplied,
                IsCredit = invoice.IsCredit,
                DueAmount = invoice.DueAmount,
                CreditDueDate = invoice.CreditDueDate,
                CreatedAt = invoice.CreatedAt,
                Items = items.Select(item => new SalesInvoiceItemDTO
                {
                    Id = item.Id,
                    PartId = item.PartId,
                    UnitPrice = item.UnitPrice,
                    PartQuantity = item.PartQuantity,
                    SubTotal = item.SubTotal,
                    CreatedAt = item.CreatedAt
                }).ToList()
            };
        }

        public async Task<SalesInvoiceDTO?> GetByIdAsync(int id)
        {
            var invoice = await _salesInvoiceRepository.GetByIdAsync(id);
            if (invoice is null)
                return null;

            var items = await _salesInvoiceRepository.GetItemsByInvoiceIdAsync(invoice.Id);

            return new SalesInvoiceDTO
            {
                Id = invoice.Id,
                CustomerId = invoice.CustomerId,
                StaffId = invoice.StaffId,
                TotalAmount = invoice.TotalAmount,
                LoyaltyApplied = invoice.LoyaltyApplied,
                IsCredit = invoice.IsCredit,
                DueAmount = invoice.DueAmount,
                CreditDueDate = invoice.CreditDueDate,
                CreatedAt = invoice.CreatedAt,
                Items = items.Select(item => new SalesInvoiceItemDTO
                {
                    Id = item.Id,
                    PartId = item.PartId,
                    UnitPrice = item.UnitPrice,
                    PartQuantity = item.PartQuantity,
                    SubTotal = item.SubTotal,
                    CreatedAt = item.CreatedAt
                }).ToList()
            };
        }
    }
}
