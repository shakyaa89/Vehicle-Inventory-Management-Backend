using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class PurchaseInvoiceService(IPurchaseInvoiceRepository purchaseInvoiceRepository) : IPurchaseInvoiceService
    {
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository = purchaseInvoiceRepository;

        public async Task<PurchaseInvoiceDTO?> CreateAsync(PurchaseInvoiceDTO invoiceData, long userId)
        {
            var vendorExists = await _purchaseInvoiceRepository.VendorExistsAsync(invoiceData.VendorId);
            if (!vendorExists)
                return null;

            var partIds = invoiceData.Items.Select(i => i.PartId).Distinct().ToList();
            var parts = await _purchaseInvoiceRepository.GetPartsByIdsAsync(partIds);

            if (parts.Count != partIds.Count)
                return null;

            var partsById = parts.ToDictionary(part => part.Id);
            var now = DateTime.UtcNow;
            var items = new List<PurchaseInvoiceItem>();
            decimal totalAmount = 0;

            foreach (var item in invoiceData.Items)
            {
                var part = partsById[item.PartId];
                part.StockQuantity += item.PartQuantity;

                var subTotal = item.UnitPrice * item.PartQuantity;
                totalAmount += subTotal;

                items.Add(new PurchaseInvoiceItem
                {
                    PartId = item.PartId,
                    UnitPrice = item.UnitPrice,
                    PartQuantity = item.PartQuantity,
                    SubTotal = subTotal,
                    CreatedAt = now
                });
            }

            var invoice = new PurchaseInvoice
            {
                VendorId = invoiceData.VendorId,
                UserId = userId,
                TotalAmount = totalAmount,
                CreatedAt = now
            };

            foreach (var item in items)
            {
                item.PurchaseInvoice = invoice;
            }

            await _purchaseInvoiceRepository.CreateAsync(invoice, items);

            return new PurchaseInvoiceDTO
            {
                Id = invoice.Id,
                VendorId = invoice.VendorId,
                UserId = invoice.UserId,
                TotalAmount = invoice.TotalAmount,
                CreatedAt = invoice.CreatedAt,
                Items = items.Select(item => new PurchaseInvoiceItemDTO
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

        public async Task<PurchaseInvoiceDTO?> GetByIdAsync(int id)
        {
            var invoice = await _purchaseInvoiceRepository.GetByIdAsync(id);
            if (invoice is null)
                return null;

            var items = await _purchaseInvoiceRepository.GetItemsByInvoiceIdAsync(invoice.Id);

            return new PurchaseInvoiceDTO
            {
                Id = invoice.Id,
                VendorId = invoice.VendorId,
                UserId = invoice.UserId,
                TotalAmount = invoice.TotalAmount,
                CreatedAt = invoice.CreatedAt,
                Items = items.Select(item => new PurchaseInvoiceItemDTO
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
