using HotelManagement.API.Modules.ReviewModule.DTOs;
using HotelManagement.Common.Models;
using HotelManagement.API.Modules.ReviewModule.Repositories;
using HotelManagement.API.Modules.ReviewModule.Validators;
using FluentValidation;

namespace HotelManagement.API.Modules.ReviewModule.Services;

public class ReviewService(IReviewRepository reviewRepository, IValidator<ReviewCreateDto> createValidator, IValidator<ReviewUpdateDto> updateValidator) : IReviewService
{
    public async Task<List<ReviewDto>> GetAllAsync() => (await reviewRepository.GetAllAsync()).Select(Map).ToList();
    public async Task<ReviewDto?> GetByIdAsync(int id) => (await reviewRepository.GetByIdAsync(id)) is { } r ? Map(r) : null;
    public async Task<List<ReviewDto>> GetByReservationIdAsync(int reservationId) => (await reviewRepository.GetByReservationIdAsync(reservationId)).Select(Map).ToList();
    public async Task<List<ReviewDto>> GetByRatingAsync(int rating) => (await reviewRepository.GetByRatingAsync(rating)).Select(Map).ToList();
    public async Task<List<ReviewDto>> GetLatestAsync() => (await reviewRepository.GetLatestAsync()).Select(Map).ToList();
    public Task<double> GetAverageRatingAsync() => reviewRepository.GetAverageRatingAsync();
    public async Task<List<ReviewDto>> GetTopRatedAsync() => (await reviewRepository.GetTopRatedAsync()).Select(Map).ToList();

    public async Task<ReviewDto> CreateAsync(ReviewCreateDto dto)
    {
        createValidator.ValidateAndThrow(dto);
        var review = await reviewRepository.AddAsync(new Review
        {
            ReservationId = dto.ReservationId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            ReviewDate = DateOnly.FromDateTime(DateTime.UtcNow)
        });
        return Map(review);
    }

    public async Task<bool> UpdateAsync(int id, ReviewUpdateDto dto)
    {
        updateValidator.ValidateAndThrow(dto);
        var existing = await reviewRepository.GetByIdAsync(id);
        if (existing is null) return false;
        existing.Rating = dto.Rating;
        existing.Comment = dto.Comment;
        await reviewRepository.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await reviewRepository.GetByIdAsync(id);
        if (existing is null) return false;
        await reviewRepository.DeleteAsync(existing);
        return true;
    }

    private static ReviewDto Map(Review r) => new()
    {
        ReviewId = r.ReviewId,
        ReservationId = r.ReservationId ?? 0,
        Rating = r.Rating ?? 0,
        Comment = r.Comment,
        ReviewDate = r.ReviewDate,
        CreatedAt = r.ReviewDate?.ToDateTime(TimeOnly.MinValue)
    };
}
