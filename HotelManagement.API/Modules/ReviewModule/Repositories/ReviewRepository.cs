using HotelManagement.Common.Data;
using HotelManagement.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Modules.ReviewModule.Repositories;

public class ReviewRepository(HotelDbContext context) : IReviewRepository
{
    public Task<List<Review>> GetAllAsync() =>
        context.Reviews.AsNoTracking().OrderByDescending(x => x.ReviewDate).ToListAsync();

    public Task<Review?> GetByIdAsync(int id) => context.Reviews.AsNoTracking().FirstOrDefaultAsync(x => x.ReviewId == id);

    public Task<List<Review>> GetByReservationIdAsync(int reservationId) =>
        context.Reviews.AsNoTracking().Where(x => x.ReservationId == reservationId).ToListAsync();

    public Task<List<Review>> GetByRatingAsync(int rating) =>
        context.Reviews.AsNoTracking().Where(x => x.Rating == rating).ToListAsync();

    public Task<List<Review>> GetLatestAsync(int takeCount = 10) =>
        context.Reviews.AsNoTracking().OrderByDescending(x => x.ReviewDate).Take(takeCount).ToListAsync();

    public async Task<double> GetAverageRatingAsync() =>
        await context.Reviews.AnyAsync(x => x.Rating.HasValue)
            ? await context.Reviews.Where(x => x.Rating.HasValue).AverageAsync(x => (double?)x.Rating) ?? 0
            : 0;

    public Task<List<Review>> GetTopRatedAsync(int takeCount = 10) =>
        context.Reviews.AsNoTracking().OrderByDescending(x => x.Rating).ThenByDescending(x => x.ReviewDate).Take(takeCount).ToListAsync();

    public async Task<Review> AddAsync(Review review)
    {
        context.Reviews.Add(review);
        await context.SaveChangesAsync();
        return review;
    }

    public async Task UpdateAsync(Review review)
    {
        context.Reviews.Update(review);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Review review)
    {
        context.Reviews.Remove(review);
        await context.SaveChangesAsync();
    }

    public async Task<List<Review>> GetAllWithGuestDetailsAsync() =>
        await context.Reviews.OrderByDescending(x => x.ReviewDate)
            .ToListAsync();
}
