using HotelManagement.API.Modules.AmenityModule.DTOs;
using HotelManagement.Common.Models;
using HotelManagement.API.Modules.AmenityModule.Repositories;
using HotelManagement.API.Modules.AmenityModule.Validators;
using FluentValidation;

namespace HotelManagement.API.Modules.AmenityModule.Services;

public class AmenityService(IAmenityRepository amenityRepository, IValidator<AmenityCreateDto> validator) : IAmenityService
{
    public async Task<List<AmenityDto>> GetAllAsync() => (await amenityRepository.GetAllAsync()).Select(MapAmenity).ToList();

    public async Task<AmenityDto?> GetByIdAsync(int id) =>
        (await amenityRepository.GetByIdAsync(id)) is { } amenity ? MapAmenity(amenity) : null;

    public async Task<List<AmenityDto>> SearchByNameAsync(string name) =>
        (await amenityRepository.SearchByNameAsync(name)).Select(MapAmenity).ToList();

    public async Task<List<object>> GetHotelsByAmenityAsync(int id) =>
        (await amenityRepository.GetHotelsByAmenityIdAsync(id)).Select(x => (object)new { x.HotelId, x.Name }).ToList();

    public async Task<List<object>> GetRoomsByAmenityAsync(int id) =>
        (await amenityRepository.GetRoomsByAmenityIdAsync(id)).Select(x => (object)new { x.RoomId, x.RoomNumber, x.RoomTypeId }).ToList();

    public async Task<List<AmenityDto>> GetHotelOnlyAsync() =>
        (await amenityRepository.GetHotelOnlyAmenitiesAsync()).Select(MapAmenity).ToList();

    public async Task<List<AmenityDto>> GetRoomOnlyAsync() =>
        (await amenityRepository.GetRoomOnlyAmenitiesAsync()).Select(MapAmenity).ToList();

    public async Task<AmenityDto> CreateAsync(AmenityCreateDto dto)
    {
        validator.ValidateAndThrow(dto);
        var amenity = await amenityRepository.AddAsync(new Amenity { Name = dto.Name, Description = dto.Description });
        return MapAmenity(amenity);
    }

    public async Task<bool> UpdateAsync(int id, AmenityCreateDto dto)
    {
        validator.ValidateAndThrow(dto);
        var existing = await amenityRepository.GetByIdAsync(id);
        if (existing is null) return false;
        existing.Name = dto.Name;
        existing.Description = dto.Description;
        await amenityRepository.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await amenityRepository.GetByIdAsync(id);
        if (existing is null) return false;
        await amenityRepository.DeleteAsync(existing);
        return true;
    }

    private static AmenityDto MapAmenity(Amenity a) => new()
    {
        AmenityId = a.AmenityId,
        Name = a.Name ?? string.Empty,
        Description = a.Description
    };
}
