using HotelManagement.API.Modules.ReservationModule.DTOs;

namespace HotelManagement.API.Modules.ReservationModule.Services;

public interface IReservationService
{
    Task<IEnumerable<ReservationDetailsDto>?> GetAllAsync();
    Task<ReservationDetailsDto> GetReservationDetailsAsync(int reservationId);
    Task<ReservationDetailsDto> CreateReservationAsync(CreateReservationDto reservationDto);
    Task<bool> HasReservationDateConflictAsync(int roomId, DateOnly checkIn, DateOnly checkOut);
    Task<List<ReservationDetailsDto>> GetReservationsByUserEmailAsync(string userEmail);
}