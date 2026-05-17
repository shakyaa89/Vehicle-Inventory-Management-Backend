using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Exceptions;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;
using Microsoft.Extensions.Logging;

namespace VehicleIMS_backend.Infrastructure.Services
{
    // Service for managing appointment operations
    public class AppointmentService(IAppointmentRepository appointmentRepository, ILogger<AppointmentService> logger) : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository = appointmentRepository;
        private readonly ILogger<AppointmentService> _logger = logger;

        // Get all appointments, optionally filtered by search term
        public async Task<IEnumerable<AppointmentResponseDTO>> GetAllAsync(string? searchTerm = null)
        {
            _logger.LogInformation("Fetching all appointments with search term {SearchTerm}", searchTerm);
            return await _appointmentRepository.GetAllAsync(searchTerm);
        }

        // Get appointments for a specific customer
        public async Task<IEnumerable<AppointmentResponseDTO>> GetByCustomerIdAsync(long customerId)
        {
            _logger.LogInformation("Fetching appointments for customer {CustomerId}", customerId);
            return await _appointmentRepository.GetByCustomerIdAsync(customerId);
        }

        // Get a single appointment by id
        public async Task<AppointmentResponseDTO?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching appointment {AppointmentId}", id);
            return await _appointmentRepository.GetByIdAsync(id) ??
                throw new NotFoundException("Appointment not found!");
        }

        // Create a new appointment
        public async Task<AppointmentResponseDTO> AddAppointmentAsync(AppointmentDTO appointmentData)
        {
            _logger.LogInformation(
                "Creating appointment for customer {CustomerId} and vehicle {VehicleId}",
                appointmentData.CustomerId,
                appointmentData.VehicleId);
            await ValidateReferencesAsync(appointmentData);

            var appointment = new Appointment
            {
                CustomerId = appointmentData.CustomerId,
                ScheduledAt = appointmentData.ScheduledAt,
                Status = appointmentData.Status,
                VehicleId = appointmentData.VehicleId,
            };

            var createdAppointment = await _appointmentRepository.AddAppointmentAsync(appointment);
            return await _appointmentRepository.GetByIdAsync(createdAppointment.Id) ??
                throw new NotFoundException("Appointment not found!");
        }

        // Update an existing appointment
        public async Task<AppointmentResponseDTO?> UpdateAsync(int id, AppointmentDTO appointmentData)
        {
            _logger.LogInformation("Updating appointment {AppointmentId}", id);
            var existingAppointment = await _appointmentRepository.GetEntityByIdAsync(id);

            if (existingAppointment is null)
                throw new NotFoundException("Appointment not found!");

            await ValidateReferencesAsync(appointmentData);

            existingAppointment.CustomerId = appointmentData.CustomerId;
            existingAppointment.ScheduledAt = appointmentData.ScheduledAt;
            existingAppointment.Status = appointmentData.Status;
            existingAppointment.VehicleId = appointmentData.VehicleId;

            await _appointmentRepository.UpdateAppointmentAsync(existingAppointment);
            return await _appointmentRepository.GetByIdAsync(existingAppointment.Id) ??
                throw new NotFoundException("Appointment not found!");
        }

        // Delete an appointment by id
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting appointment {AppointmentId}", id);
            var existingAppointment = await _appointmentRepository.GetEntityByIdAsync(id);

            if (existingAppointment is null)
                throw new NotFoundException("Appointment not found!");

            await _appointmentRepository.DeleteAsync(existingAppointment);
            return true;
        }

        // Mark appointment as completed
        public async Task<AppointmentResponseDTO?> CompleteAsync(int id)
        {
            _logger.LogInformation("Marking appointment {AppointmentId} as completed", id);
            return await UpdateStatusAsync(id, "Completed");
        }

        // Cancel an appointment
        public async Task<AppointmentResponseDTO?> CancelAsync(int id)
        {
            _logger.LogInformation("Cancelling appointment {AppointmentId}", id);
            return await UpdateStatusAsync(id, "Cancelled");
        }

        // Internal helper to update appointment status
        private async Task<AppointmentResponseDTO?> UpdateStatusAsync(int id, string status)
        {
            var existingAppointment = await _appointmentRepository.GetEntityByIdAsync(id);

            if (existingAppointment is null)
                throw new NotFoundException("Appointment not found!");

            existingAppointment.Status = status;

            await _appointmentRepository.UpdateAppointmentAsync(existingAppointment);
            return await _appointmentRepository.GetByIdAsync(existingAppointment.Id) ??
                throw new NotFoundException("Appointment not found!");
        }

        // Ensure referenced customer and vehicle exist
        private async Task ValidateReferencesAsync(AppointmentDTO appointmentData)
        {
            var customerExists = await _appointmentRepository.CustomerExistsAsync(appointmentData.CustomerId);
            if (!customerExists)
                throw new NotFoundException("Customer does not exist.");

            var vehicleExists = await _appointmentRepository.VehicleExistsAsync(appointmentData.VehicleId);
            if (!vehicleExists)
                throw new NotFoundException("Vehicle does not exist.");
        }
    }
}