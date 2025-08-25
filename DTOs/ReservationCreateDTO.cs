using System.ComponentModel.DataAnnotations;

namespace GXReservationAPI.DTOs
{
    public class ReservationCreateDTO
    {
        public string Name { get; set; }
        public string Purpose { get; set; }
        public int RoomId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly TimeStart { get; set; }
        public TimeSpan TimeDuration { get; set; }
    }
}
