using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/vendors")]
    [ApiController]
    // Controller for vendor endpoints
    public class VendorController(IVendorService vendorService, ILogger<VendorController> logger) : ControllerBase
    {
        private readonly IVendorService _vendorService = vendorService;
        private readonly ILogger<VendorController> _logger = logger;

        // Get all vendors
        [HttpGet]
        public async Task<IActionResult> GetAllVendors()
        {
            _logger.LogInformation("Fetching all vendors");
            var vendors = await _vendorService.GetAllAsync();
            return Ok(vendors);
        }

        // Get a single vendor by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVendorById(int id)
        {
            _logger.LogInformation("Fetching vendor {VendorId}", id);
            var vendor = await _vendorService.GetByIdAsync(id);

            if (vendor is null)
                return NotFound(new { message = "Vendor not found" });

            return Ok(vendor);
        }

        // Create a new vendor
        [HttpPost]
        public async Task<IActionResult> AddVendor(VendorDTO vendorData)
        {
            _logger.LogInformation("Creating vendor {VendorName}", vendorData.Name);
            var vendor = await _vendorService.AddAsync(vendorData);

            return CreatedAtAction(nameof(GetVendorById), new { id = vendor.Id }, vendor);
        }

        // Update an existing vendor
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateVendor(int id, VendorDTO vendorData)
        {
            _logger.LogInformation("Updating vendor {VendorId}", id);
            var vendor = await _vendorService.UpdateAsync(id, vendorData);

            if (vendor is null)
                return NotFound(new { message = "Vendor not found" });

            return Ok(vendor);
        }

        // Delete a vendor
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            _logger.LogInformation("Deleting vendor {VendorId}", id);
            var deleted = await _vendorService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Vendor not found" });

            return Ok(new { message = "Vendor deleted successfully" });
        }
    }
}
