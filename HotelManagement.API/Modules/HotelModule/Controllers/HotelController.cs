using FluentValidation;
using HotelManagement.API.Modules.AmenityModule.DTOs;
using HotelManagement.API.Modules.HotelModule.DTOs;
using HotelManagement.API.Modules.HotelModule.Services;
using HotelManagement.API.Modules.RoomModule.DTOs;
using HotelManagement.Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.API.Exceptions;


namespace HotelManagement.API.Modules.HotelModule.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelController(
    IHotelService service,
    IValidator<HotelDto> createValidator,
    IValidator<HotelDto> updateValidator) : ControllerBase
{
    private readonly IHotelService _service = service;
    private readonly IValidator<HotelDto> _createValidator = createValidator;
    private readonly IValidator<HotelDto> _updateValidator = updateValidator;

    // GET /api/hotels
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var hotels = await _service.GetAllAsync();
        var response = new ApiResponse<IEnumerable<HotelResponseDto>>
        {
            Success = true,
            StatusCode = 200,
            Message = "Successfully fetched hotels.",
            Data = hotels,
            Errors = null,
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }

    // GET /api/hotels/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var hotel = await _service.GetByIdAsync(id);
        var response = new ApiResponse<HotelResponseDto>
        {
            Success = true,
            StatusCode = 200,
            Message = "Successfully fetched hotel details.",
            Data = hotel,
            Errors = null,
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }

   // POST /api/hotels
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HotelDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return UnprocessableEntity(ApiResponse<object>.Fail(422, "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage).ToList()));
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.HotelId },
                ApiResponse<HotelResponseDto>.Ok(200, "Hotel created successfully.", created));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ApiResponse<HotelResponseDto>.Fail(400, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<HotelResponseDto>.Fail(500, "An unexpected error occurred.", new List<string> { ex.Message }));
        }
    }


    // PUT /api/hotels/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] HotelDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return UnprocessableEntity(ApiResponse<object>.Fail(422, "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage).ToList()));
        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<HotelResponseDto>.Ok(200, "Hotel updated successfully.", updated));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<HotelResponseDto>.Fail(404, ex.Message));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ApiResponse<HotelResponseDto>.Fail(400, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<HotelResponseDto>.Fail(500, "An unexpected error occurred.", new List<string> { ex.Message }));
        }
    }

    // DELETE /api/hotels/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(200, "Hotel deleted successfully.", null));
    }

    // GET /api/hotels/search?location={location}
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return BadRequest(ApiResponse<object>.Fail(400, "Location query parameter is required."));

        var hotels = await _service.SearchByLocationAsync(location);
        return Ok(ApiResponse<IEnumerable<HotelResponseDto>>.Ok(200, "Success", hotels));
    }

    // GET /api/hotels/{id}/rooms
    [HttpGet("{id:int}/rooms")]
    public async Task<IActionResult> GetRooms(int id)
    {
        var rooms = await _service.GetRoomsAsync(id);
        return Ok(ApiResponse<IEnumerable<RoomBriefDto>>.Ok(200, "Success", rooms));
    }

    // GET /api/hotels/{id}/rooms
    [HttpGet("{id:int}/amenities")]
    public async Task<IActionResult> GetAmenities(int id)
    {
        var amenities = await _service.GetAmenitiesAsync(id);
        return Ok(ApiResponse<IEnumerable<AmenityDto>>.Ok(200, "Success", amenities));
    }

    // GET /api/hotels/{id}/available-rooms
    [HttpGet("{id:int}/available-rooms")]
    public async Task<IActionResult> GetAvailableRooms(int id)
    {
        var rooms = await _service.GetAvailableRoomsWithPriceAsync(id);
        return Ok(ApiResponse<IEnumerable<RoomWithPriceDto>>.Ok(200, "Success", rooms));
    }

    // GET /api/hotels/{id}/reservations
    [HttpGet("{id:int}/reservations")]
    public async Task<IActionResult> GetReservations(int id)
    {
        var reservations = await _service.GetReservationsAsync(id);
        return Ok(ApiResponse<IEnumerable<ReservationBriefDto>>.Ok(200, "Success", reservations));
    }

    // GET /api/hotels/{id}/summary
    [HttpGet("{id:int}/summary")]
    public async Task<IActionResult> GetSummary(int id)
    {
        var summary = await _service.GetSummaryAsync(id);
        return Ok(ApiResponse<HotelSummaryDto>.Ok(200, "Success", summary));
    }
}
