using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Authorize(Roles = "Staff")]
    [Route("api/sales-invoices")]
    [ApiController]
    public class SalesInvoiceController(ISalesInvoiceService salesInvoiceService) : ControllerBase
    {
        private readonly ISalesInvoiceService _salesInvoiceService = salesInvoiceService;

        [HttpPost]
        public async Task<IActionResult> CreateInvoice(SalesInvoiceDTO invoiceData)
        {
            if (invoiceData.Items is null || invoiceData.Items.Count == 0)
                return BadRequest(new { message = "At least one item is required." });

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(new { message = "Invalid token payload" });

            var invoice = await _salesInvoiceService.CreateAsync(invoiceData, userId);

            if (invoice is null)
                return BadRequest(new { message = "Unable to create sales invoice. Check customer, parts, and stock." });

            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await _salesInvoiceService.GetByIdAsync(id);

            if (invoice is null)
                return NotFound(new { message = "Sales invoice not found" });

            return Ok(invoice);
        }
    }
}
