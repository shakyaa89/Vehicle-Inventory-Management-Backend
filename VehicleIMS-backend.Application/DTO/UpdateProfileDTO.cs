using System.ComponentModel.DataAnnotations;

namespace VehicleIMS_backend.Application.DTO
{
    public class UpdateProfileDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
