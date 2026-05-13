using System.Threading.Tasks;

namespace VehicleIMS_backend.Application.Interfaces.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
