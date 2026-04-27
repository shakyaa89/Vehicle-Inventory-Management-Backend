using Microsoft.AspNetCore.Mvc;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/vehicles")]
    [ApiController]
    public class VehicleController(IVehicleService vehicleService) : ControllerBase
    {
        private readonly IVehicleService _vehicleService = vehicleService;

        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllAsync();
            return Ok(vehicles);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);

            if (vehicle is null)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle(VehicleDTO vehicleDTO)
        {
            var vehicle = await _vehicleService.AddAsync(vehicleDTO);

            return CreatedAtAction(nameof(GetVehicleById), new { id = vehicle.Id }, vehicle);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateVehicle(int id, VehicleDTO vehicleDTO)
        {
            var vehicle = await _vehicleService.UpdateAsync(id, vehicleDTO);

            if (vehicle is null)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(vehicle);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var deleted = await _vehicleService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(new { message = "Vehicle deleted successfully" });
        }
    }
}
