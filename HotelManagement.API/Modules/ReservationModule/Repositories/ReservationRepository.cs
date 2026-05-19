using HotelManagement.Common.Data;
using HotelManagement.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Modules.ReservationModule.Repositories;

public class ReservationRepository(HotelDbContext dbContext) : IReservationRepository
{
    private readonly HotelDbContext _dbContext = dbContext;

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId) =>
        await _dbContext.Reservations.FindAsync(reservationId);

    public async Task<Reservation> CreateReservationAsync(Reservation reservation)
    {
        await _dbContext.AddAsync(reservation);
        await _dbContext.SaveChangesAsync();
        return reservation;
    }

    public async Task<bool> HasReservationDateConflictAsync(int roomId, DateOnly checkIn, DateOnly checkOut) =>
        await _dbContext.Reservations
            .AnyAsync(r => 
                r.RoomId == roomId &&
                checkIn < r.CheckOutDate &&
                checkOut > r.CheckInDate);

    public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId) =>
        await _dbContext.Reservations
            .Where(r => r.ApplicationUsers.Any(u => u.Id == userId))
            .ToListAsync();

    public async Task<IEnumerable<Reservation>?> GetAllAsync() =>
        await _dbContext.Reservations.ToListAsync();
}