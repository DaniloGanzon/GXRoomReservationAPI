
using System.ComponentModel.DataAnnotations;

namespace GXReservationAPI.Model
{
    public class ReservationModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Purpose { get; set; }
        [Required]
        public int RoomId { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        [Required]
        public TimeOnly TimeStart { get; set; }
        [Required]
        public TimeSpan TimeDuration { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Status { get; set; } 
    }
}
