using HotelManagement.Common.Data;
using HotelManagement.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Modules.HotelModule.Repositories;

public class HotelRepository(HotelDbContext context) : IHotelRepository
{
    public async Task<IEnumerable<Hotel>> GetAllAsync()
        => await context.Hotels
            .GroupBy(h => h.Name)
            .Select(g => g.First())
            .ToListAsync();

    public async Task<Hotel?> GetByIdAsync(int id)
        => await context.Hotels.FindAsync(id);

    public async Task<Hotel> CreateAsync(Hotel hotel)
    {
        context.Hotels.Add(hotel);
        await context.SaveChangesAsync();
        return hotel;
    }

    public async Task<Hotel> UpdateAsync(Hotel hotel)
    {
        context.Hotels.Update(hotel);
        await context.SaveChangesAsync();
        return hotel;
    }

    public async Task DeleteAsync(Hotel hotel)
    {
        context.Hotels.Remove(hotel);
        await context.SaveChangesAsync(); 
    }

    public async Task<IEnumerable<Hotel>> SearchByLocationAsync(string location)
        => await context.Hotels
            .Where(h => h.Location != null && h.Location.ToLower().Contains(location.ToLower()))
            .ToListAsync();

    public async Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(int hotelId)
        => await context.Rooms
            .GroupBy(r => r.RoomType)
            .Select(g => g.First())
            .ToListAsync();

    public async Task<IEnumerable<Room>> GetAvailableRoomsByHotelIdAsync(int hotelId)
        => await context.Rooms
            .Where(r => r.IsAvailable == true)
            .GroupBy(r => r.RoomType)
            .Select(g => g.First())
            .ToListAsync();

    public async Task<IEnumerable<Reservation>> GetReservationsByHotelIdAsync(int hotelId)
        => await context.Reservations
            .Include(r => r.Room)
            .ThenInclude(room => room != null ? room.RoomType : null)
            .ToListAsync();

    public async Task<IEnumerable<Amenity>> GetAmenitiesByHotelIdAsync(int hotelId)
        => await context.Amenities
            .Where(a => a.Hotels.Any(h => h.HotelId == hotelId))
            .ToListAsync();
}
