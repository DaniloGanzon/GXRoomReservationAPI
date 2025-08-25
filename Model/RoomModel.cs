using System.ComponentModel.DataAnnotations;
using GXReservationAPI.Constants;

namespace GXReservationAPI.Model
{
    public class RoomModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Building { get; set; }
        [Required]
        public string Floor { get; set; }
        [Required]
        public string Status { get; set; } 
    }
}
