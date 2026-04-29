using Microsoft.AspNetCore.Mvc;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/vendors")]
    [ApiController]
    public class VendorController(IVendorService vendorService) : ControllerBase
    {
        private readonly IVendorService _vendorService = vendorService;

        [HttpGet]
        public async Task<IActionResult> GetAllVendors()
        {
            var vendors = await _vendorService.GetAllAsync();
            return Ok(vendors);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVendorById(int id)
        {
            var vendor = await _vendorService.GetByIdAsync(id);

            if (vendor is null)
                return NotFound(new { message = "Vendor not found" });

            return Ok(vendor);
        }

        [HttpPost]
        public async Task<IActionResult> AddVendor(VendorDTO vendorData)
        {
            var vendor = await _vendorService.AddAsync(vendorData);

            return CreatedAtAction(nameof(GetVendorById), new { id = vendor.Id }, vendor);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateVendor(int id, VendorDTO vendorData)
        {
            var vendor = await _vendorService.UpdateAsync(id, vendorData);

            if (vendor is null)
                return NotFound(new { message = "Vendor not found" });

            return Ok(vendor);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var deleted = await _vendorService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Vendor not found" });

            return Ok(new { message = "Vendor deleted successfully" });
        }
    }
}
