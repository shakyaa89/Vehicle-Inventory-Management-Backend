using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IJwtTokenService
    {
        Task<String> GenerateUserToken(User user);
    }
}
