using HotelManagement.API.Exceptions;
using HotelManagement.API.Modules.HotelModule.DTOs;
using HotelManagement.API.Modules.HotelModule.Repositories;
using HotelManagement.API.Modules.AmenityModule.Repositories;
using HotelManagement.Common.Models;
using HotelManagement.API.Modules.RoomModule.DTOs;
using HotelManagement.API.Modules.AmenityModule.DTOs;

namespace HotelManagement.API.Modules.HotelModule.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepo;
    private readonly IAmenityRepository _amenityRepo;

    public HotelService(IHotelRepository hotelRepo, IAmenityRepository amenityRepo)
    {
        _hotelRepo = hotelRepo;
        _amenityRepo = amenityRepo;
    }

    public async Task<IEnumerable<HotelResponseDto>> GetAllAsync()
    {
        var hotels = await _hotelRepo.GetAllAsync();
        return hotels.Select(MapToResponse);
    }

    public async Task<HotelResponseDto> GetByIdAsync(int id)
    {
        var hotel = await _hotelRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Hotel with id {id} was not found.");
        return MapToResponse(hotel);
    }

    public async Task<HotelResponseDto> CreateAsync(HotelDto dto)
    {
        var hotel = new Hotel
        {
            Name = dto.Name,
            Location = dto.Location,
            Description = dto.Description
        };
        var created = await _hotelRepo.CreateAsync(hotel);
        return MapToResponse(created);
    }

    public async Task<HotelResponseDto> UpdateAsync(int id, HotelDto dto)
    {
        var hotel = await _hotelRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Hotel with id {id} was not found.");

        hotel.Name = dto.Name ?? hotel.Name;
        hotel.Location = dto.Location ?? hotel.Location;
        hotel.Description = dto.Description ?? hotel.Description;

        var updated = await _hotelRepo.UpdateAsync(hotel);
        return MapToResponse(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var hotel = await _hotelRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Hotel with id {id} was not found.");
        await _hotelRepo.DeleteAsync(hotel);
    }

    public async Task<IEnumerable<HotelResponseDto>> SearchByLocationAsync(string location)
    {
        var hotels = await _hotelRepo.SearchByLocationAsync(location);
        return hotels.Select(MapToResponse);
    }

    public async Task<IEnumerable<RoomBriefDto>> GetRoomsAsync(int hotelId)
    {
        _ = await _hotelRepo.GetByIdAsync(hotelId) ?? throw new NotFoundException($"Hotel with id {hotelId} was not found.");
        var rooms = await _hotelRepo.GetRoomsByHotelIdAsync(hotelId);
        return rooms.Select(MapToRoomBrief);
    }

    public async Task<IEnumerable<RoomBriefDto>> GetAvailableRoomsAsync(int hotelId)
    {
        _ = await _hotelRepo.GetByIdAsync(hotelId) ?? throw new NotFoundException($"Hotel with id {hotelId} was not found.");
        var rooms = await _hotelRepo.GetAvailableRoomsByHotelIdAsync(hotelId);
        return rooms.Select(MapToRoomBrief);
    }

    public async Task<IEnumerable<ReservationBriefDto>> GetReservationsAsync(int hotelId)
    {
        _ = await _hotelRepo.GetByIdAsync(hotelId) ?? throw new NotFoundException($"Hotel with id {hotelId} was not found.");
        var reservations = await _hotelRepo.GetReservationsByHotelIdAsync(hotelId);
        return reservations.Select(r => new ReservationBriefDto
        {
            ReservationId = r.ReservationId,
            GuestName = r.GuestName,
            GuestEmail = r.GuestEmail,
            CheckInDate = r.CheckInDate,
            CheckOutDate = r.CheckOutDate,
            RoomNumber = r.Room?.RoomNumber
        });
    }

    public async Task<HotelSummaryDto> GetSummaryAsync(int hotelId)
    {
        var hotel = await _hotelRepo.GetByIdAsync(hotelId)
            ?? throw new NotFoundException($"Hotel with id {hotelId} was not found.");
        var rooms = (await _hotelRepo.GetRoomsByHotelIdAsync(hotelId)).ToList();
        var availableRooms = rooms.Count(r => r.IsAvailable == true);
        var reservations = await _hotelRepo.GetReservationsByHotelIdAsync(hotelId);
        var amenityCount = await _amenityRepo.CountByHotelIdAsync(hotelId);

        return new HotelSummaryDto
        {
            HotelId = hotel.HotelId,
            Name = hotel.Name,
            Location = hotel.Location,
            TotalRooms = rooms.Count,
            AvailableRooms = availableRooms,
            TotalReservations = reservations.Count(),
            TotalAmenities = amenityCount
        };
    }

    private static HotelResponseDto MapToResponse(Hotel h) => new()
    {
        HotelId = h.HotelId,
        Name = h.Name,
        Location = h.Location,
        Description = h.Description
    };

    private static RoomBriefDto MapToRoomBrief(Room r) => new()
    {
        RoomId = r.RoomId,
        RoomNumber = r.RoomNumber,
        IsAvailable = r.IsAvailable,
        RoomTypeName = r.RoomType?.TypeName,
        PricePerNight = r.RoomType?.PricePerNight
    };

    public async Task<IEnumerable<RoomWithPriceDto>> GetAvailableRoomsWithPriceAsync(int hotelId)
    {
        var rooms = await _hotelRepo.GetAvailableRoomsByHotelIdAsync(hotelId) ??
            throw new NotFoundException();

        return rooms.Select(r => new RoomWithPriceDto
        {
            RoomId = r.RoomId,
            RoomType = r.RoomType!.TypeName ?? "",
            Price = r.RoomType!.PricePerNight ?? 0m,
            MaxOccupancy = r.RoomType!.MaxOccupancy ?? 0
        });
    }

    public async Task<IEnumerable<AmenityDto>> GetAmenitiesAsync(int hotelId)
    {
        var amenities = await _hotelRepo.GetAmenitiesByHotelIdAsync(hotelId) ??
            throw new NotFoundException();

        return amenities.Select(a => new AmenityDto
        {
            AmenityId = a.AmenityId,
            Name = a.Name ?? "",
            Description = a.Description!
        });
    }
}
