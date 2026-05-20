using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Authorize]
    [Route("api/sales-invoices")]
    [ApiController]
    // Controller for sales invoice endpoints
    public class SalesInvoiceController(ISalesInvoiceService salesInvoiceService, ILogger<SalesInvoiceController> logger) : ControllerBase
    {
        private readonly ISalesInvoiceService _salesInvoiceService = salesInvoiceService;
        private readonly ILogger<SalesInvoiceController> _logger = logger;

        // Create a new sales invoice
        [HttpPost]
        public async Task<IActionResult> CreateInvoice(SalesInvoiceDTO invoiceData)
        {
            _logger.LogInformation("Creating sales invoice for customer {CustomerId}", invoiceData.CustomerId);
            if (invoiceData.Items is null || invoiceData.Items.Count == 0)
                return BadRequest(new { message = "At least one item is required." });

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(new { message = "Invalid token payload" });

            // Ensure customer id matches token for customers
            var isCustomer = string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase);
            if (isCustomer)
            {
                invoiceData.CustomerId = userId;
            }
            else if (invoiceData.CustomerId <= 0)
            {
                return BadRequest(new { message = "Customer is required." });
            }

            var invoice = await _salesInvoiceService.CreateAsync(invoiceData, userId);

            if (invoice is null)
                return BadRequest(new { message = "Unable to create sales invoice. Check customer, parts, and stock." });

            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
        }

        // Get all sales invoices
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            _logger.LogInformation("Fetching all sales invoices");
            var invoices = await _salesInvoiceService.GetAllAsync();
            return Ok(invoices);
        }

        // Send a sales invoice email
        [HttpPost("{id:int}/send-email")]
        public async Task<IActionResult> SendInvoiceEmail(int id)
        {
            _logger.LogInformation("Sending sales invoice email for invoice {InvoiceId}", id);
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(new { message = "Invalid token payload" });

            await _salesInvoiceService.SendInvoiceEmailAsync(id, userId);

            return Ok(new { message = "Invoice email sent." });
        }

        // Get invoices for a specific customer
        [HttpGet("customer/{customerId:long}")]
        public async Task<IActionResult> GetInvoicesByCustomer(long customerId)
        {
            _logger.LogInformation("Fetching sales invoices for customer {CustomerId}", customerId);
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(new { message = "Invalid token payload" });

            // Restrict customers to their own invoices
            var isCustomer = string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase);
            if (isCustomer && userId != customerId)
                return Forbid();

            var invoices = await _salesInvoiceService.GetByCustomerIdAsync(customerId);
            return Ok(invoices);
        }

        // Get a single sales invoice by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            _logger.LogInformation("Fetching sales invoice {InvoiceId}", id);
            var invoice = await _salesInvoiceService.GetByIdAsync(id);

            if (invoice is null)
                return NotFound(new { message = "Sales invoice not found" });

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(new { message = "Invalid token payload" });

            // Restrict customers to their own invoice
            var isCustomer = string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase);
            if (isCustomer && invoice.CustomerId != userId)
                return Forbid();

            return Ok(invoice);
        }
    }
}
