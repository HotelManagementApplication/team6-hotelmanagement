using HotelManagement.API.Modules.AmenityModule.DTOs;
using HotelManagement.API.Modules.HotelModule.DTOs;
using HotelManagement.API.Modules.RoomModule.DTOs;

namespace HotelManagement.API.Modules.HotelModule.Services;

public interface IHotelService
{
    Task<IEnumerable<HotelResponseDto>> GetAllAsync();
    Task<HotelResponseDto> GetByIdAsync(int id);
    Task<HotelResponseDto> CreateAsync(HotelDto dto);
    Task<HotelResponseDto> UpdateAsync(int id, HotelDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<HotelResponseDto>> SearchByLocationAsync(string location);
    Task<IEnumerable<RoomBriefDto>> GetRoomsAsync(int hotelId);
    Task<IEnumerable<AmenityDto>> GetAmenitiesAsync(int hotelId);
    Task<IEnumerable<RoomBriefDto>> GetAvailableRoomsAsync(int hotelId);
    Task<IEnumerable<RoomWithPriceDto>> GetAvailableRoomsWithPriceAsync(int hotelId);
    Task<IEnumerable<ReservationBriefDto>> GetReservationsAsync(int hotelId);
    Task<HotelSummaryDto> GetSummaryAsync(int hotelId);
}
