using HotelManagement.API.Modules.ReviewModule.DTOs;
using HotelManagement.Common.DTOs;
using HotelManagement.API.Modules.ReviewModule.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Modules.ReviewModule.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll() =>
        Ok(new ApiResponse<object> { Success = true, Message = "Reviews fetched.", Data = await reviewService.GetAllAsync() });

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await reviewService.GetByIdAsync(id);
        return item is null
            ? NotFound(new ApiResponse<object> { Success = false, Message = "Review not found." })
            : Ok(new ApiResponse<object> { Success = true, Message = "Review fetched.", Data = item });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
    {
        var created = await reviewService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.ReviewId },
            new ApiResponse<object> { Success = true, Message = "Review created.", Data = created });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] ReviewUpdateDto dto) =>
        await reviewService.UpdateAsync(id, dto)
            ? Ok(new ApiResponse<object> { Success = true, Message = "Review updated." })
            : NotFound(new ApiResponse<object> { Success = false, Message = "Review not found." });

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        await reviewService.DeleteAsync(id)
            ? Ok(new ApiResponse<object> { Success = true, Message = "Review deleted." })
            : NotFound(new ApiResponse<object> { Success = false, Message = "Review not found." });

    [HttpGet("by-reservation/{reservationId:int}")]
    [Authorize]
    public async Task<IActionResult> ByReservation(int reservationId) =>
        Ok(new ApiResponse<object> { Success = true, Message = "Reviews by reservation fetched.", Data = await reviewService.GetByReservationIdAsync(reservationId) });

    [HttpGet("by-rating/{rating:int}")]
    [Authorize]
    public async Task<IActionResult> ByRating(int rating) =>
        Ok(new ApiResponse<object> { Success = true, Message = "Reviews by rating fetched.", Data = await reviewService.GetByRatingAsync(rating) });

    [HttpGet("latest")]
    [Authorize]
    public async Task<IActionResult> Latest() =>
        Ok(new ApiResponse<object> { Success = true, Message = "Latest reviews fetched.", Data = await reviewService.GetLatestAsync() });

    [HttpGet("average-rating")]
    [Authorize]
    public async Task<IActionResult> AverageRating() =>
        Ok(new ApiResponse<object> { Success = true, Message = "Average rating fetched.", Data = await reviewService.GetAverageRatingAsync() });

    [HttpGet("top-rated")]
    [Authorize]
    public async Task<IActionResult> TopRated() =>
        Ok(new ApiResponse<object> { Success = true, Message = "Top-rated reviews fetched.", Data = await reviewService.GetTopRatedAsync() });
    
    [HttpGet("with-guest")]
    public async Task<IActionResult> WithGuest() =>
        Ok(new ApiResponse<object> { Success = true, Message = "Successfully fetched reviews.", Data = await reviewService.GetAllWithGuestDetailsAsync() });
}
