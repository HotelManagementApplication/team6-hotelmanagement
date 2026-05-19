using HotelManagement.Common.DTOs;
using HotelManagement.MVC.Services;
using HotelManagement.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.MVC.Controllers;

public class BookingsController(ApiService apiService) : Controller
{
    private readonly ApiService _apiService = apiService;
    private readonly string _endpoint = "reservation";

    [Authorize]
    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _apiService
                .GetAsync<ApiResponse<IEnumerable<BookingViewModel>>>(
                    $"{_endpoint}/my-reservations");
            
            if (response is null)
                throw new NullReferenceException();

            var bookings = response.Data;
            return View(bookings);    
        }
        catch (NullReferenceException)
        {
            ViewData["BookingsError"] = "Failed to fetch bookings for the user.";
            return View();
        }
    }
}
