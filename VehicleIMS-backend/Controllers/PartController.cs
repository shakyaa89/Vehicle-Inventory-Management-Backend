using Microsoft.AspNetCore.Mvc;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/parts")]
    [ApiController]
    public class PartController(IPartService partService) : ControllerBase
    {
        private readonly IPartService _partService = partService;

        [HttpGet]
        public async Task<IActionResult> GetAllParts()
        {
            var parts = await _partService.GetAllAsync();
            return Ok(parts);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPartById(int id)
        {
            var part = await _partService.GetByIdAsync(id);

            if (part is null)
                return NotFound(new { message = "Part not found" });

            return Ok(part);
        }

        [HttpPost]
        public async Task<IActionResult> AddPart(PartDTO partData)
        {
            var part = await _partService.AddAsync(partData);

            return CreatedAtAction(nameof(GetPartById), new { id = part.Id }, part);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePart(int id, PartDTO partData)
        {
            var part = await _partService.UpdateAsync(id, partData);

            if (part is null)
                return NotFound(new { message = "Part not found" });

            return Ok(part);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePart(int id)
        {
            var deleted = await _partService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Part not found" });

            return Ok(new { message = "Part deleted successfully" });
        }
    }
}