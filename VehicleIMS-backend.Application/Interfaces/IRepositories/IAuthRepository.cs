using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS_backend.Domain.Models;

namespace VehicleIMS_backend.Application.Interfaces.IRepositories
{
    public interface IAuthRepository
    {
        Task CreateCustomerAsync(CustomerStats customer);
    }
}
