using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IRepositories
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAsync();
        Task<List<Appointment>> GetByCustomerIdAsync(long customerId);
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> AddAppointmentAsync(Appointment appointment);
        Task<Appointment> UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAsync(Appointment appointment);
        Task<bool> CustomerExistsAsync(long customerId);
        Task<bool> VehicleExistsAsync(int vehicleId);
    }
}
