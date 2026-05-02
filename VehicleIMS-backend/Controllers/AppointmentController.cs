using Microsoft.AspNetCore.Mvc;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController(IAppointmentService appointmentService) : ControllerBase
    {
        private readonly IAppointmentService _appointmentService = appointmentService;

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("customer/{customerId:long}")]
        public async Task<IActionResult> GetAppointmentsByCustomerId(long customerId)
        {
            var appointments = await _appointmentService.GetByCustomerIdAsync(customerId);
            return Ok(appointments);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment is null)
                return NotFound(new { message = "Appointment not found" });

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> AddAppointment(AppointmentDTO appointmentData)
        {
            var appointment = await _appointmentService.AddAppointmentAsync(appointmentData);

            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAppointment(int id, AppointmentDTO appointmentData)
        {
            var appointment = await _appointmentService.UpdateAsync(id, appointmentData);

            if (appointment is null)
                return NotFound(new { message = "Appointment not found" });

            return Ok(appointment);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var deleted = await _appointmentService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Appointment not found" });

            return Ok(new { message = "Appointment deleted successfully" });
        }
    }
}