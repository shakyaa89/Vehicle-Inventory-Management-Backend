using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/vehicles")]
    [ApiController]
    // Controller for vehicle endpoints
    public class VehicleController(IVehicleService vehicleService, ILogger<VehicleController> logger) : ControllerBase
    {
        private readonly IVehicleService _vehicleService = vehicleService;
        private readonly ILogger<VehicleController> _logger = logger;

        // Get all vehicles
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            _logger.LogInformation("Fetching all vehicles");
            var vehicles = await _vehicleService.GetAllAsync();
            return Ok(vehicles);
        }

        // Get vehicles for a specific customer
        [HttpGet("customer/{customerId:long}")]
        public async Task<IActionResult> GetVehiclesByCustomerId(long customerId)
        {
            _logger.LogInformation("Fetching vehicles for customer {CustomerId}", customerId);
            var vehicles = await _vehicleService.GetByCustomerIdAsync(customerId);
            return Ok(vehicles);
        }

        // Get a single vehicle by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            _logger.LogInformation("Fetching vehicle {VehicleId}", id);
            var vehicle = await _vehicleService.GetByIdAsync(id);

            if (vehicle is null)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(vehicle);
        }

        // Create a new vehicle
        [HttpPost]
        public async Task<IActionResult> AddVehicle(VehicleDTO vehicleDTO)
        {
            _logger.LogInformation("Creating vehicle for customer {CustomerId}", vehicleDTO.CustomerId);
            var vehicle = await _vehicleService.AddAsync(vehicleDTO);

            return CreatedAtAction(nameof(GetVehicleById), new { id = vehicle.Id }, vehicle);
        }

        // Update an existing vehicle
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateVehicle(int id, VehicleDTO vehicleDTO)
        {
            _logger.LogInformation("Updating vehicle {VehicleId}", id);
            var vehicle = await _vehicleService.UpdateAsync(id, vehicleDTO);

            if (vehicle is null)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(vehicle);
        }

        // Delete a vehicle
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            _logger.LogInformation("Deleting vehicle {VehicleId}", id);
            var deleted = await _vehicleService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(new { message = "Vehicle deleted successfully" });
        }
    }
}
