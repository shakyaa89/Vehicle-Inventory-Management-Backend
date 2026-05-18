using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Exceptions;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;
using Microsoft.Extensions.Logging;

namespace VehicleIMS_backend.Infrastructure.Services
{
    // Service to handle purchase invoice creation and retrieval
    public class PurchaseInvoiceService(IPurchaseInvoiceRepository purchaseInvoiceRepository, ILogger<PurchaseInvoiceService> logger) : IPurchaseInvoiceService
    {
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository = purchaseInvoiceRepository;
        private readonly ILogger<PurchaseInvoiceService> _logger = logger;

        // Create a purchase invoice and update part stock
        public async Task<PurchaseInvoiceDTO?> CreateAsync(PurchaseInvoiceDTO invoiceData, long userId)
        {
            _logger.LogInformation("Creating purchase invoice for vendor {VendorId} by user {UserId}", invoiceData.VendorId, userId);
            if (invoiceData.Items is null || invoiceData.Items.Count == 0)
                throw new BadRequestException("Invoice items are required.");

            var vendorExists = await _purchaseInvoiceRepository.VendorExistsAsync(invoiceData.VendorId);
            if (!vendorExists)
                throw new NotFoundException("Vendor not found.");

            var partIds = invoiceData.Items.Select(i => i.PartId).Distinct().ToList();
            var parts = await _purchaseInvoiceRepository.GetPartsByIdsAsync(partIds);

            if (parts.Count != partIds.Count)
                throw new NotFoundException("One or more parts not found.");

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

        // Get purchase invoice by id
        public async Task<PurchaseInvoiceDTO?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching purchase invoice {InvoiceId}", id);
            var invoice = await _purchaseInvoiceRepository.GetByIdAsync(id);
            if (invoice is null)
                throw new NotFoundException("Purchase invoice not found.");

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

        // Get all purchase invoices (summary)
        public async Task<List<PurchaseInvoiceDTO>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all purchase invoices");
            var invoices = await _purchaseInvoiceRepository.GetAllAsync();

            return invoices.Select(inv => new PurchaseInvoiceDTO
            {
                Id = inv.Id,
                VendorId = inv.VendorId,
                UserId = inv.UserId,
                TotalAmount = inv.TotalAmount,
                CreatedAt = inv.CreatedAt,
                Items = new List<PurchaseInvoiceItemDTO>()
            }).ToList();
        }
    }
}
