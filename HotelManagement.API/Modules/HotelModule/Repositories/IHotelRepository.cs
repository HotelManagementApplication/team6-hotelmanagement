using HotelManagement.Common.Models;

namespace HotelManagement.API.Modules.HotelModule.Repositories;

public interface IHotelRepository
{
    Task<IEnumerable<Hotel>> GetAllAsync();
    Task<Hotel?> GetByIdAsync(int id);
    Task<Hotel> CreateAsync(Hotel hotel);
    Task<Hotel> UpdateAsync(Hotel hotel);
    Task DeleteAsync(Hotel hotel);
    Task<IEnumerable<Hotel>> SearchByLocationAsync(string location);
    Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(int hotelId);
    Task<IEnumerable<Amenity>> GetAmenitiesByHotelIdAsync(int hotelId);
    Task<IEnumerable<Room>> GetAvailableRoomsByHotelIdAsync(int hotelId);
    Task<IEnumerable<Reservation>> GetReservationsByHotelIdAsync(int hotelId);
}
