using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IRepositories
{
    public interface IAppointmentRepository
    {
        Task<List<AppointmentResponseDTO>> GetAllAsync(string? searchTerm = null);
        Task<List<AppointmentResponseDTO>> GetByCustomerIdAsync(long customerId);
        Task<AppointmentResponseDTO?> GetByIdAsync(int id);
        Task<Appointment?> GetEntityByIdAsync(int id);
        Task<Appointment> AddAppointmentAsync(Appointment appointment);
        Task<Appointment> UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAsync(Appointment appointment);
        Task<bool> CustomerExistsAsync(long customerId);
        Task<bool> VehicleExistsAsync(int vehicleId);
    }
}
