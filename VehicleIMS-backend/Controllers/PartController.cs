using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/parts")]
    [ApiController]
    // Controller for part endpoints
    public class PartController(IPartService partService, ILogger<PartController> logger) : ControllerBase
    {
        private readonly IPartService _partService = partService;
        private readonly ILogger<PartController> _logger = logger;

        // Get all parts
        [HttpGet]
        public async Task<IActionResult> GetAllParts()
        {
            _logger.LogInformation("Fetching all parts");
            var parts = await _partService.GetAllAsync();
            return Ok(parts);
        }

        // Get a single part by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPartById(int id)
        {
            _logger.LogInformation("Fetching part {PartId}", id);
            var part = await _partService.GetByIdAsync(id);

            if (part is null)
                return NotFound(new { message = "Part not found" });

            return Ok(part);
        }

        // Create a new part
        [HttpPost]
        public async Task<IActionResult> AddPart(PartDTO partData)
        {
            _logger.LogInformation("Creating part {PartName} with SKU {Sku}", partData.Name, partData.Sku);
            var part = await _partService.AddAsync(partData);

            return CreatedAtAction(nameof(GetPartById), new { id = part.Id }, part);
        }

        // Update an existing part
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePart(int id, PartDTO partData)
        {
            _logger.LogInformation("Updating part {PartId}", id);
            var part = await _partService.UpdateAsync(id, partData);

            if (part is null)
                return NotFound(new { message = "Part not found" });

            return Ok(part);
        }

        // Delete a part
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePart(int id)
        {
            _logger.LogInformation("Deleting part {PartId}", id);
            var deleted = await _partService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Part not found" });

            return Ok(new { message = "Part deleted successfully" });
        }
    }
}