using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Authorize]
    [Route("api/sales-invoices")]
    [ApiController]
    public class SalesInvoiceController(ISalesInvoiceService salesInvoiceService) : ControllerBase
    {
        private readonly ISalesInvoiceService _salesInvoiceService = salesInvoiceService;

        [Authorize(Roles = "Staff,Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateInvoice(SalesInvoiceDTO invoiceData)
        {
            if (invoiceData.Items is null || invoiceData.Items.Count == 0)
                return BadRequest(new { message = "At least one item is required." });

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(new { message = "Invalid token payload" });

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

        [Authorize(Roles = "Staff,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _salesInvoiceService.GetAllAsync();
            return Ok(invoices);
        }

        [Authorize(Roles = "Staff,Admin")]
        [HttpPost("{id:int}/send-email")]
        public async Task<IActionResult> SendInvoiceEmail(int id)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(new { message = "Invalid token payload" });

            await _salesInvoiceService.SendInvoiceEmailAsync(id, userId);

            return Ok(new { message = "Invoice email sent." });
        }

        [Authorize(Roles = "Staff,Admin,Customer")]
        [HttpGet("customer/{customerId:long}")]
        public async Task<IActionResult> GetInvoicesByCustomer(long customerId)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(new { message = "Invalid token payload" });

            var isCustomer = string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase);
            if (isCustomer && userId != customerId)
                return Forbid();

            var invoices = await _salesInvoiceService.GetByCustomerIdAsync(customerId);
            return Ok(invoices);
        }

        [Authorize(Roles = "Staff,Admin,Customer")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await _salesInvoiceService.GetByIdAsync(id);

            if (invoice is null)
                return NotFound(new { message = "Sales invoice not found" });

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(new { message = "Invalid token payload" });

            var isCustomer = string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase);
            if (isCustomer && invoice.CustomerId != userId)
                return Forbid();

            return Ok(invoice);
        }
    }
}
