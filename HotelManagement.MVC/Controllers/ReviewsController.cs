using HotelManagement.Common.DTOs;
using HotelManagement.MVC.Services;
using HotelManagement.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.MVC.Controllers;

public class ReviewsController(ApiService apiService) : Controller
{
    private readonly ApiService _apiService = apiService;

    public async Task<IActionResult> Index()
    {
        var reviewResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<ReviewDetailsViewModel>>>("reviews/with-guest");
        var reviews = reviewResponse?.Data;

        return View(reviews);
    }
}