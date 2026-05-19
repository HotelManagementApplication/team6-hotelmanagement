using HotelManagement.API.DTOs;
using HotelManagement.API.Modules.AmenityModule.DTOs;
using HotelManagement.API.Modules.AmenityModule.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Modules.AmenityModule.Controllers;

[ApiController]
[Route("api/amenities")]
public class AmenityController(IAmenityService amenityService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(new ApiResponse<object> { Success = true, Message = "Amenities fetched.", Data = await amenityService.GetAllAsync() });

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await amenityService.GetByIdAsync(id);
        return item is null
            ? NotFound(new ApiResponse<object> { Success = false, Message = "Amenity not found." })
            : Ok(new ApiResponse<object> { Success = true, Message = "Amenity fetched.", Data = item });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] AmenityCreateDto dto)
    {
        var created = await amenityService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.AmenityId },
            new ApiResponse<object> { Success = true, Message = "Amenity created.", Data = created });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] AmenityCreateDto dto) =>
        await amenityService.UpdateAsync(id, dto)
            ? Ok(new ApiResponse<object> { Success = true, Message = "Amenity updated." })
            : NotFound(new ApiResponse<object> { Success = false, Message = "Amenity not found." });

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        await amenityService.DeleteAsync(id)
            ? Ok(new ApiResponse<object> { Success = true, Message = "Amenity deleted." })
            : NotFound(new ApiResponse<object> { Success = false, Message = "Amenity not found." });

    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> Search([FromQuery] string name) =>
        Ok(new ApiResponse<object> { Success = true, Message = "Search results fetched.", Data = await amenityService.SearchByNameAsync(name) });

    [HttpGet("{id:int}/hotels")]
    [Authorize]
    public async Task<IActionResult> GetHotelsByAmenity(int id) =>
        Ok(new ApiResponse<object> { Success = true, Message = "Hotels fetched.", Data = await amenityService.GetHotelsByAmenityAsync(id) });

    [HttpGet("{id:int}/rooms")]
    [Authorize]
    public async Task<IActionResult> GetRoomsByAmenity(int id) =>
        Ok(new ApiResponse<object> { Success = true, Message = "Rooms fetched.", Data = await amenityService.GetRoomsByAmenityAsync(id) });

    [HttpGet("hotel-only")]
    [Authorize]
    public async Task<IActionResult> GetHotelOnly() =>
        Ok(new ApiResponse<object> { Success = true, Message = "Hotel-only amenities fetched.", Data = await amenityService.GetHotelOnlyAsync() });

    [HttpGet("room-only")]
    [Authorize]
    public async Task<IActionResult> GetRoomOnly() =>
        Ok(new ApiResponse<object> { Success = true, Message = "Room-only amenities fetched.", Data = await amenityService.GetRoomOnlyAsync() });
}
