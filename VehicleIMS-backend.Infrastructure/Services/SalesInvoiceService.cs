using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Exceptions;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Services
{
    // Service for creating and sending sales invoices
    public class SalesInvoiceService(ISalesInvoiceRepository salesInvoiceRepository, IEmailService emailService, UserManager<User> userManager, ILogger<SalesInvoiceService> logger) : ISalesInvoiceService
    {
        private readonly ISalesInvoiceRepository _salesInvoiceRepository = salesInvoiceRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<SalesInvoiceService> _logger = logger;

        // Create a sales invoice and update stock
        public async Task<SalesInvoiceDTO?> CreateAsync(SalesInvoiceDTO invoiceData, long staffId)
        {
            _logger.LogInformation("Creating sales invoice for customer {CustomerId} by staff {StaffId}", invoiceData.CustomerId, staffId);
            if (invoiceData.Items is null || invoiceData.Items.Count == 0)
                throw new BadRequestException("Invoice items are required.");

            var customerExists = await _salesInvoiceRepository.CustomerExistsAsync(invoiceData.CustomerId);
            if (!customerExists)
                throw new NotFoundException("Customer not found.");

            var customer = await _salesInvoiceRepository.GetCustomerByUserIdAsync(invoiceData.CustomerId)
                ?? throw new NotFoundException("Customer not found.");

            var partIds = invoiceData.Items.Select(i => i.PartId).Distinct().ToList();
            var parts = await _salesInvoiceRepository.GetPartsByIdsAsync(partIds);

            if (parts.Count != partIds.Count)
                throw new NotFoundException("One or more parts not found.");

            var partsById = parts.ToDictionary(part => part.Id);
            var now = DateTime.UtcNow;
            var items = new List<SalesInvoiceItem>();
            decimal totalAmount = 0;

            foreach (var item in invoiceData.Items)
            {
                if (item.PartQuantity <= 0 || item.UnitPrice < 0)
                    throw new BadRequestException("Invalid item quantity or unit price.");

                if (!partsById.TryGetValue(item.PartId, out var part))
                    throw new NotFoundException("Part not found.");

                if (part.StockQuantity < item.PartQuantity)
                    throw new BadRequestException("Insufficient stock for requested part.");

                part.StockQuantity -= item.PartQuantity;

                if (part.StockQuantity < 10 && part.StockQuantity + item.PartQuantity >= 10)
                {
                    await SendLowStockEmailAsync(part);
                }

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

            if(totalAmount >= 5000){
                totalAmount = totalAmount - (totalAmount * 0.1m);
            }

            var dueAmount = Convert.ToInt32(Math.Round(totalAmount));

            if (invoiceData.IsCredit && customer.CreditBalance + dueAmount > 3000)
            {
                throw new BadRequestException("Customer credit balance limit of 3000 has been exceeded.");
            }

            var creditDueDate = invoiceData.IsCredit ? now.Date.AddMonths(1) : (DateTime?)null;

            var invoice = new SalesInvoice
            {
                CustomerId = invoiceData.CustomerId,
                StaffId = staffId,
                TotalAmount = totalAmount,
                LoyaltyApplied = invoiceData.LoyaltyApplied,
                IsCredit = invoiceData.IsCredit,
                DueAmount = invoiceData.IsCredit ? dueAmount : 0,
                CreditDueDate = creditDueDate,
                CreatedAt = now
            };

            if (invoiceData.IsCredit)
            {
                customer.CreditBalance += dueAmount;
            }

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

        // Get sales invoice by id
        public async Task<SalesInvoiceDTO?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching sales invoice {InvoiceId}", id);
            var invoice = await _salesInvoiceRepository.GetByIdAsync(id);
            if (invoice is null)
                throw new NotFoundException("Invoice not found.");

            var items = await _salesInvoiceRepository.GetItemsByInvoiceIdAsync(invoice.Id);

            return MapInvoice(invoice, items);
        }

        // Get all sales invoices
        public async Task<List<SalesInvoiceDTO>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all sales invoices");
            var invoices = await _salesInvoiceRepository.GetAllAsync();
            if (invoices.Count == 0)
                return new List<SalesInvoiceDTO>();

            var results = new List<SalesInvoiceDTO>(invoices.Count);

            foreach (var invoice in invoices)
            {
                var items = await _salesInvoiceRepository.GetItemsByInvoiceIdAsync(invoice.Id);
                results.Add(MapInvoice(invoice, items));
            }

            return results;
        }

        // Get sales invoices for a customer
        public async Task<List<SalesInvoiceDTO>> GetByCustomerIdAsync(long customerId)
        {
            _logger.LogInformation("Fetching sales invoices for customer {CustomerId}", customerId);
            var invoices = await _salesInvoiceRepository.GetByCustomerIdAsync(customerId);
            if (invoices.Count == 0)
                return new List<SalesInvoiceDTO>();

            var results = new List<SalesInvoiceDTO>(invoices.Count);

            foreach (var invoice in invoices)
            {
                var items = await _salesInvoiceRepository.GetItemsByInvoiceIdAsync(invoice.Id);
                results.Add(MapInvoice(invoice, items));
            }

            return results;
        }

        // Build and send invoice email to customer
        public async Task SendInvoiceEmailAsync(int invoiceId, long staffId)
        {
            _logger.LogInformation("Sending sales invoice email for invoice {InvoiceId}", invoiceId);
            var invoice = await _salesInvoiceRepository.GetByIdAsync(invoiceId) ?? throw new NotFoundException("Invoice not found");

            var customer = await _salesInvoiceRepository.GetUserByIdAsync(invoice.CustomerId) ?? throw new NotFoundException("Customer not found");

            if (string.IsNullOrWhiteSpace(customer.Email)) throw new BadRequestException("Customer email is missing");

            var items = await _salesInvoiceRepository.GetItemsByInvoiceIdAsync(invoice.Id);

            if (items.Count == 0) throw new BadRequestException("Invoice has no items");

            var partIds = items.Select(x => x.PartId).Distinct().ToList();

            var parts = partIds.Count == 0 ? new List<Part>() : await _salesInvoiceRepository.GetPartsByIdsAsync(partIds);

            var partsById = parts.ToDictionary(x => x.Id);

            var staff = await _salesInvoiceRepository.GetUserByIdAsync(staffId);

            var subject = $"Sales Invoice #{invoice.Id}";
            var body = BuildInvoiceEmailBody(invoice, items, partsById, customer, staff);

            await _emailService.SendEmailAsync(customer.Email, subject, body);
        }

        private async Task SendLowStockEmailAsync(Part part)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var adminRecipients = admins
                .Where(user => !string.IsNullOrWhiteSpace(user.Email))
                .ToList();

            if (adminRecipients.Count == 0)
            {
                _logger.LogWarning("Low stock alert skipped for part {PartId} because no admin email was found", part.Id);
                return;
            }

            var subject = $"Low stock alert: {part.Name}";
            var body = $"""
                <html>
                <body>
                    <h2>Low Stock Alert</h2>
                    <p><strong>Part:</strong> {WebUtility.HtmlEncode(part.Name)}</p>
                    <p><strong>SKU:</strong> {WebUtility.HtmlEncode(part.Sku)}</p>
                    <p><strong>Remaining stock:</strong> {part.StockQuantity}</p>
                </body>
                </html>
                """;

            foreach (var admin in adminRecipients)
            {
                await _emailService.SendEmailAsync(admin.Email!, subject, body);
            }

            _logger.LogInformation("Low stock alert sent for part {PartId} to {RecipientCount} admins", part.Id, adminRecipients.Count);
        }

        // Map domain invoice and items to DTO
        private static SalesInvoiceDTO MapInvoice(SalesInvoice invoice, List<SalesInvoiceItem> items)
        {
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

        private static string BuildInvoiceEmailBody(
            SalesInvoice invoice,
            IReadOnlyList<SalesInvoiceItem> items,
            IReadOnlyDictionary<int, Part> partsById,
            User customer,
            User? staff)
        {
            string customerName = WebUtility.HtmlEncode(!string.IsNullOrWhiteSpace(customer.FullName) ? customer.FullName : !string.IsNullOrWhiteSpace(customer.UserName) ? customer.UserName : customer.Email ?? "Customer");

            string staffName = staff is null ? string.Empty : WebUtility.HtmlEncode(!string.IsNullOrWhiteSpace(staff.FullName) ? staff.FullName : !string.IsNullOrWhiteSpace(staff.UserName) ? staff.UserName : staff.Email ?? "");

            var sb = new StringBuilder();

            sb.Append($"""
                <!DOCTYPE html>
                <html>
                <body>

                <h2>Sales Invoice</h2>

                <p>
                <strong>Invoice #:</strong> {invoice.Id}<br/>
                <strong>Date:</strong> {invoice.CreatedAt:yyyy-MM-dd HH:mm} UTC<br/>
                <strong>Customer:</strong> {customerName}
                </p>

                <table style="border-collapse:collapse;width:100%">
                <thead>
                <tr>
                <th style="text-align:left;border-bottom:1px solid #ccc;padding:6px 4px;">Part</th>
                <th style="text-align:right;border-bottom:1px solid #ccc;padding:6px 4px;">Qty</th>
                <th style="text-align:right;border-bottom:1px solid #ccc;padding:6px 4px;">Unit Price</th>
                <th style="text-align:right;border-bottom:1px solid #ccc;padding:6px 4px;">Subtotal</th>
                </tr>
                </thead>
                <tbody>
                """);

                    foreach (var item in items)
                    {
                        var partLabel = partsById.TryGetValue(item.PartId, out var part)
                            ? $"{part.Name} ({part.Sku})"
                            : $"Part #{item.PartId}";

                        sb.Append($"""
                <tr>
                <td style="padding:4px;">{WebUtility.HtmlEncode(partLabel)}</td>
                <td style="padding:4px;text-align:right;">{item.PartQuantity}</td>
                <td style="padding:4px;text-align:right;">{item.UnitPrice:0.00}</td>
                <td style="padding:4px;text-align:right;">{item.SubTotal:0.00}</td>
                </tr>
                """);
                    }

                    sb.Append($"""
                </tbody>
                </table>

                <p>
                <strong>Total:</strong> {invoice.TotalAmount:0.00}
                </p>
                """);

                    if (invoice.IsCredit)
                    {
                        sb.Append($"""
                <p>
                <strong>Due Amount:</strong> {invoice.DueAmount:0.00}<br/>
                <strong>Due Date:</strong> {invoice.CreditDueDate?.ToString("yyyy-MM-dd") ?? "N/A"}
                </p>
                """);
                    }

                    if (!string.IsNullOrWhiteSpace(staffName))
                    {
                        sb.Append($"""
                <p><strong>Prepared by:</strong> {staffName}</p>
                """);
                    }

                    sb.Append("""
                </body>
                </html>
                """);

            return sb.ToString();
        }
    }
}
