using System.Text.Json;
using HotelManagement.Common.DTOs;
using HotelManagement.MVC.Services;
using HotelManagement.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.MVC.Controllers;

public class AdminController(ApiService apiService) : Controller
{
    private readonly ApiService _apiService = apiService;

    public async Task<IActionResult> Index()
    {
        var amenityResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<AmenityViewModel>>>("amenities");
        var amenities = amenityResponse?.Data;

        var hotelResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<HotelCardViewModel>>>("hotels");
        var hotels = hotelResponse?.Data;

        var paymentResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<PaymentDetailViewModel>>>("payment");
        var payments = paymentResponse?.Data;

        var reservationResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<ReservationViewModel>>>("reservation");
        var reservations = reservationResponse?.Data;

        var reviewResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<ReviewViewModel>>>("reviews");
        var reviews = reviewResponse?.Data;

        var roomResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<RoomViewModel>>>("rooms");
        var rooms = roomResponse?.Data;

        var roomTypeResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<RoomTypeViewModel>>>("roomtypes");
        var roomTypes = roomTypeResponse?.Data;

        var model = new AdminDashboardViewModel
        {
            Amenities = amenities ?? [],
            Hotels = hotels ?? [],
            Payments = payments ?? [],
            Reservations = reservations ?? [],
            Reviews = reviews ?? [],
            Rooms = rooms ?? [],
            RoomTypes = roomTypes ?? []
        };
        return View(model);
    }
}