using GXReservationAPI.Constants;
using GXReservationAPI.Data;
using GXReservationAPI.Model;
using GXReservationAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace GXReservationAPI.Services
{
    public class ReservationService : IRepostory<ReservationModel>
    {
        private readonly AppDbContext _context;

        public ReservationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReservationModel reservation)
        {
            await _context.Reservation.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reservation = await GetByIdAsync(id) ??
                throw new KeyNotFoundException($"Reservation id {id} not found");

            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReservationModel>> GetAllAsync()
        {
            return await _context.Reservation.ToListAsync();
        }

        public async Task<ReservationModel?> GetByIdAsync(int id)
        {
            return await _context.Reservation.FindAsync(id);
        }

        public async Task UpdateAsync(ReservationModel reservation)
        {
            _context.Reservation.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Reservation.AnyAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<ReservationModel>> GetByRoomIdAsync(int roomId)
        {
            return await _context.Reservation
                .Where(r => r.RoomId == roomId)
                .ToListAsync();
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateOnly startDate, DateOnly endDate, TimeOnly timeStart, TimeSpan duration)
        {
            var reservations = await _context.Reservation
                .Where(r => r.RoomId == roomId && r.Status != ReservationStatus.Rejected)
                .ToListAsync();
            foreach (var reservation in reservations)
            {
                if (startDate <= reservation.EndDate && endDate >= reservation.StartDate)
                {
                    var requestedEndTime = timeStart.Add(duration);
                    var reservationEndTime = reservation.TimeStart.Add(reservation.TimeDuration);

                    if (timeStart < reservationEndTime && requestedEndTime > reservation.TimeStart)
                    {
                        return false;
                    }
                }
            }
            return true; 
        }

    }
}
