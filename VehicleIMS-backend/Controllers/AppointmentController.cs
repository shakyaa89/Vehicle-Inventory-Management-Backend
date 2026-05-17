using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController(IAppointmentService appointmentService, ILogger<AppointmentController> logger) : ControllerBase
    {
        private readonly IAppointmentService _appointmentService = appointmentService;
        private readonly ILogger<AppointmentController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments([FromQuery] string? search = null)
        {
            _logger.LogInformation("Fetching all appointments with search term {SearchTerm}", search);
            var appointments = await _appointmentService.GetAllAsync(search);
            return Ok(appointments);
        }

        [HttpGet("customer/{customerId:long}")]
        public async Task<IActionResult> GetAppointmentsByCustomerId(long customerId)
        {
            _logger.LogInformation("Fetching appointments for customer {CustomerId}", customerId);
            var appointments = await _appointmentService.GetByCustomerIdAsync(customerId);
            return Ok(appointments);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            _logger.LogInformation("Fetching appointment {AppointmentId}", id);
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment is null)
                return NotFound(new { message = "Appointment not found" });

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> AddAppointment(AppointmentDTO appointmentData)
        {
            _logger.LogInformation("Creating appointment for customer {CustomerId} and vehicle {VehicleId}", appointmentData.CustomerId, appointmentData.VehicleId);
            var appointment = await _appointmentService.AddAppointmentAsync(appointmentData);

            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAppointment(int id, AppointmentDTO appointmentData)
        {
            _logger.LogInformation("Updating appointment {AppointmentId}", id);
            var appointment = await _appointmentService.UpdateAsync(id, appointmentData);

            if (appointment is null)
                return NotFound(new { message = "Appointment not found" });

            return Ok(appointment);
        }

        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            _logger.LogInformation("Marking appointment {AppointmentId} as completed", id);
            var appointment = await _appointmentService.CompleteAsync(id);

            if (appointment is null)
                return NotFound(new { message = "Appointment not found" });

            return Ok(appointment);
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            _logger.LogInformation("Cancelling appointment {AppointmentId}", id);
            var appointment = await _appointmentService.CancelAsync(id);

            if (appointment is null)
                return NotFound(new { message = "Appointment not found" });

            return Ok(appointment);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            _logger.LogInformation("Deleting appointment {AppointmentId}", id);
            var deleted = await _appointmentService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Appointment not found" });

            return Ok(new { message = "Appointment deleted successfully" });
        }
    }
}