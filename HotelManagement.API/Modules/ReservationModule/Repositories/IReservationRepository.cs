using HotelManagement.Common.Models;

namespace HotelManagement.API.Modules.ReservationModule.Repositories;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>?> GetAllAsync();
    Task<Reservation?> GetReservationByIdAsync(int reservationId);
    Task<Reservation> CreateReservationAsync(Reservation reservation);
    Task<bool> HasReservationDateConflictAsync(int roomId, DateOnly checkIn, DateOnly checkOut);
    Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId);
}