using HotelManagement.Common.Models;

namespace HotelManagement.API.Modules.ReviewModule.Repositories;

public interface IReviewRepository
{
    Task<List<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(int id);
    Task<List<Review>> GetByReservationIdAsync(int reservationId);
    Task<List<Review>> GetByRatingAsync(int rating);
    Task<List<Review>> GetLatestAsync(int takeCount = 10);
    Task<double> GetAverageRatingAsync();
    Task<List<Review>> GetTopRatedAsync(int takeCount = 10);
    Task<Review> AddAsync(Review review);
    Task UpdateAsync(Review review);
    Task DeleteAsync(Review review);
    Task<List<Review>> GetAllWithGuestDetailsAsync();
}
