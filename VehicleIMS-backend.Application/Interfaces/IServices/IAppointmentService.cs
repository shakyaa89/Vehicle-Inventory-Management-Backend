using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<IEnumerable<Appointment>> GetByCustomerIdAsync(long customerId);
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> AddAppointmentAsync(AppointmentDTO appointmentData);
        Task<Appointment?> UpdateAsync(int id, AppointmentDTO appointmentData);
        Task<bool> DeleteAsync(int id);
    }
}