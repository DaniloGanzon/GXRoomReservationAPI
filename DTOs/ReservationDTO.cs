using GXReservationAPI.Constants;

namespace GXReservationAPI.DTOs
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public int RoomId { get; set; } 
        public RoomDTO? Room { get; set; }  
        public DateOnly StartDate { get; set; }     
        public DateOnly EndDate { get; set; }       
        public TimeOnly TimeStart { get; set; }     
        public TimeSpan TimeDuration { get; set; }  
        public string Status { get; set; } = ReservationStatus.Pending;
    }
}
