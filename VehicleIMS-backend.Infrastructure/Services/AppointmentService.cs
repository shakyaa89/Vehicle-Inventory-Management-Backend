using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class AppointmentService(IAppointmentRepository appointmentRepository) : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository = appointmentRepository;

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _appointmentRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByCustomerIdAsync(long customerId)
        {
            return await _appointmentRepository.GetByCustomerIdAsync(customerId);
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _appointmentRepository.GetByIdAsync(id);
        }

        public async Task<Appointment> AddAppointmentAsync(AppointmentDTO appointmentData)
        {
            await ValidateReferencesAsync(appointmentData);

            var appointment = new Appointment
            {
                CustomerId = appointmentData.CustomerId,
                ScheduledAt = appointmentData.ScheduledAt,
                Status = appointmentData.Status,
                VehicleId = appointmentData.VehicleId,
            };

            return await _appointmentRepository.AddAppointmentAsync(appointment);
        }

        public async Task<Appointment?> UpdateAsync(int id, AppointmentDTO appointmentData)
        {
            var existingAppointment = await _appointmentRepository.GetByIdAsync(id);

            if (existingAppointment is null)
                return null;

            await ValidateReferencesAsync(appointmentData);

            existingAppointment.CustomerId = appointmentData.CustomerId;
            existingAppointment.ScheduledAt = appointmentData.ScheduledAt;
            existingAppointment.Status = appointmentData.Status;
            existingAppointment.VehicleId = appointmentData.VehicleId;

            return await _appointmentRepository.UpdateAppointmentAsync(existingAppointment);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingAppointment = await _appointmentRepository.GetByIdAsync(id);

            if (existingAppointment is null)
                return false;

            await _appointmentRepository.DeleteAsync(existingAppointment);
            return true;
        }

        public async Task<Appointment?> CompleteAsync(int id)
        {
            return await UpdateStatusAsync(id, "Completed");
        }

        public async Task<Appointment?> CancelAsync(int id)
        {
            return await UpdateStatusAsync(id, "Cancelled");
        }

        private async Task<Appointment?> UpdateStatusAsync(int id, string status)
        {
            var existingAppointment = await _appointmentRepository.GetByIdAsync(id);

            if (existingAppointment is null)
                return null;

            existingAppointment.Status = status;

            return await _appointmentRepository.UpdateAppointmentAsync(existingAppointment);
        }

        private async Task ValidateReferencesAsync(AppointmentDTO appointmentData)
        {
            var customerExists = await _appointmentRepository.CustomerExistsAsync(appointmentData.CustomerId);
            if (!customerExists)
                throw new Exception("Customer does not exist.");

            var vehicleExists = await _appointmentRepository.VehicleExistsAsync(appointmentData.VehicleId);
            if (!vehicleExists)
                throw new Exception("Vehicle does not exist.");
        }
    }
}