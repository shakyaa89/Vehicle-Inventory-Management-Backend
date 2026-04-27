using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<CustomerStats> RegisterCustomer(RegisterDTO registerDTO);

        Task<User> RegisterStaff(RegisterDTO registerDTO);

        Task<object> Login(LoginDTO loginDTO);
    }
}
