using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GXReservationAPI.Model;

namespace GXReservationAPI.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<RoomModel> Room { get; set; }
        public DbSet<ReservationModel> Reservation { get; set; }
        public IEnumerable<object> Reservations { get; internal set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
