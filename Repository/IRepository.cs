namespace GXReservationAPI.Repository
{
    public interface IRepostory<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> IsRoomAvailableAsync(int roomId, DateOnly startDate, DateOnly endDate,TimeOnly timeStart, TimeSpan duration);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<T>> GetByRoomIdAsync(int roomId);
    }
}
