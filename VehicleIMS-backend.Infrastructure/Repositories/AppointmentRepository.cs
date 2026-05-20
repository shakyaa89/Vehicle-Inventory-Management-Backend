using Microsoft.EntityFrameworkCore;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IRepositories;
using VehicleIMS_backend.Domain.Models;
using VehicleIMS_backend.Infrastructure.Persistence;

namespace VehicleIMS_backend.Infrastructure.Repositories
{
    // Repository for appointment data access
    public class AppointmentRepository(AppDbContext context) : IAppointmentRepository
    {
        private readonly AppDbContext _context = context;

        // Get all appointments with optional search filtering
        public async Task<List<AppointmentResponseDTO>> GetAllAsync(string? searchTerm = null)
        {
            var query = _context.Appointments.Include(a => a.Customer).Include(a => a.Vehicle).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Normalize search term and filter by customer or vehicle fields
                var normalizedSearchTerm = searchTerm.Trim().ToLower();
                query = query.Where(a =>
                    (a.Customer != null && a.Customer.FullName.ToLower().Contains(normalizedSearchTerm)) ||
                    (a.Vehicle != null && (
                        (a.Vehicle.Make ?? string.Empty).ToLower().Contains(normalizedSearchTerm) ||
                        (a.Vehicle.Model ?? string.Empty).ToLower().Contains(normalizedSearchTerm) 
                    )));
            }

            return await query.Select(a => new AppointmentResponseDTO
                {
                    Id = a.Id,
                    CustomerId = a.CustomerId,
                    CustomerName = a.Customer!.FullName,
                    ScheduledAt = a.ScheduledAt,
                    Status = a.Status,
                    VehicleId = a.VehicleId,
                    VehicleMake = a.Vehicle!.Year + " " + a.Vehicle!.Make + " " + a.Vehicle!.Model
                }).ToListAsync();
        }

        // Get appointments for a specific customer
        public async Task<List<AppointmentResponseDTO>> GetByCustomerIdAsync(long customerId)
        {
            return await _context.Appointments.Include(a => a.Customer).Include(a => a.Vehicle).Where(a => a.CustomerId == customerId).Select(a => new AppointmentResponseDTO
                {
                    Id = a.Id,
                    CustomerId = a.CustomerId,
                    CustomerName = a.Customer!.FullName,
                    ScheduledAt = a.ScheduledAt,
                    Status = a.Status,
                    VehicleId = a.VehicleId,
                    VehicleMake = a.Vehicle!.Year + " " + a.Vehicle!.Make + " " + a.Vehicle!.Model
                }).ToListAsync();
        }

        // Get a single appointment by id
        public async Task<AppointmentResponseDTO?> GetByIdAsync(int id)
        {
            return await _context.Appointments.Include(a => a.Customer).Include(a => a.Vehicle).AsNoTracking().Where(a => a.Id == id).Select(a => new AppointmentResponseDTO
                {
                    Id = a.Id,
                    CustomerId = a.CustomerId,
                    CustomerName = a.Customer!.FullName,
                    ScheduledAt = a.ScheduledAt,
                    Status = a.Status,
                    VehicleId = a.VehicleId,
                    VehicleMake = a.Vehicle!.Year + " " + a.Vehicle!.Make + " " + a.Vehicle!.Model
                }).FirstOrDefaultAsync();
        }

            // Get the appointment entity by id for updates
            public async Task<Appointment?> GetEntityByIdAsync(int id)
            {
                return await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            }

        // Create a new appointment
        public async Task<Appointment> AddAppointmentAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        // Update an existing appointment
        public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        // Delete an appointment
        public async Task DeleteAsync(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        // Check if a customer exists
        public async Task<bool> CustomerExistsAsync(long customerId)
        {
            return await _context.Users.AnyAsync(u => u.Id == customerId);
        }

        // Check if a vehicle exists
        public async Task<bool> VehicleExistsAsync(int vehicleId)
        {
            return await _context.Vehicles.AnyAsync(v => v.Id == vehicleId);
        }
    }
}