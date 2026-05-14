using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/purchase-invoices")]
    [ApiController]
    public class PurchaseInvoiceController(IPurchaseInvoiceService purchaseInvoiceService, ILogger<PurchaseInvoiceController> logger) : ControllerBase
    {
        private readonly IPurchaseInvoiceService _purchaseInvoiceService = purchaseInvoiceService;
        private readonly ILogger<PurchaseInvoiceController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> CreateInvoice(PurchaseInvoiceDTO invoiceData)
        {
            _logger.LogInformation("Creating purchase invoice for vendor {VendorId}", invoiceData.VendorId);
            if (invoiceData.Items is null || invoiceData.Items.Count == 0)
                return BadRequest(new { message = "At least one item is required." });

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(new { message = "Invalid token payload" });

            var invoice = await _purchaseInvoiceService.CreateAsync(invoiceData, userId);

            if (invoice is null)
                return NotFound(new { message = "Vendor or part not found" });

            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            _logger.LogInformation("Fetching purchase invoice {InvoiceId}", id);
            var invoice = await _purchaseInvoiceService.GetByIdAsync(id);

            if (invoice is null)
                return NotFound(new { message = "Purchase invoice not found" });

            return Ok(invoice);
        }
    }
}
