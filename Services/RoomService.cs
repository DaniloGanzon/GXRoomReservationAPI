using GXReservationAPI.Data;
using GXReservationAPI.Model;
using Microsoft.EntityFrameworkCore;
using GXReservationAPI.Repository;

namespace GXReservationAPI.Services
{
    public class RoomService : IRepostory<RoomModel>
    {
        private readonly AppDbContext _context;

        public RoomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RoomModel room)
        {
            await _context.Room.AddAsync(room);
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync(int id)
        {
            var roomInDb = await _context.Room.FindAsync(id);

            if (roomInDb == null)
            {
                throw new KeyNotFoundException($"Room id {id} not found");
            }

            _context.Room.Remove(roomInDb);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RoomModel>> GetAllAsync()
        {
            return await _context.Room.ToListAsync();
        }

        public async Task<RoomModel?> GetByIdAsync(int id)
        {
            return await _context.Room.FindAsync(id);
        }

        public async Task UpdateAsync(RoomModel room)
        {
            _context.Room.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Room.AnyAsync(r => r.Id == id);
        }

        // These methods don't make sense for RoomService, but must be implemented
        public Task<IEnumerable<RoomModel>> GetByRoomIdAsync(int roomId)
        {
            // Not applicable for Room service
            throw new NotImplementedException();
        }

        public Task<bool> IsRoomAvailableAsync(int roomId, DateOnly startDate, DateOnly endDate,
            TimeOnly timeStart, TimeSpan duration)
        {
            // Room availability checking should be in ReservationService
            throw new NotImplementedException();
        }

    }
}

