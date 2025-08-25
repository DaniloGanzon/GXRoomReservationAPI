using GXReservationAPI.Constants;

namespace GXReservationAPI.DTOs
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Status { get; set; } = RoomStatus.Available;

    }
}
