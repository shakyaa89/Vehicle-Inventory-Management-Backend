using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentResponseDTO>> GetAllAsync();
        Task<IEnumerable<AppointmentResponseDTO>> GetByCustomerIdAsync(long customerId);
        Task<AppointmentResponseDTO?> GetByIdAsync(int id);
        Task<AppointmentResponseDTO> AddAppointmentAsync(AppointmentDTO appointmentData);
        Task<AppointmentResponseDTO?> UpdateAsync(int id, AppointmentDTO appointmentData);
        Task<bool> DeleteAsync(int id);
        Task<AppointmentResponseDTO?> CompleteAsync(int id);
        Task<AppointmentResponseDTO?> CancelAsync(int id);
    }
}