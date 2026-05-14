using System.ComponentModel.DataAnnotations;

namespace VehicleIMS_backend.Application.DTO
{
    public class ReviewDTO
    {
        [Required]
        public long CustomerId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewUpdateDTO
    {
        [Required]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;
    }
}