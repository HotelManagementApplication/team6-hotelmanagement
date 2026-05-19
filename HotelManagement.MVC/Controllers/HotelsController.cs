using HotelManagement.Common.DTOs;
using HotelManagement.MVC.Services;
using HotelManagement.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.MVC.Controllers;

public class HotelsController(ApiService apiService) : Controller
{
    private readonly ApiService _apiService = apiService;
    private readonly string _hotelEndpoint = "hotels";

    public async Task<IActionResult> Index()
    {
        try
        {
            IEnumerable<HotelCardViewModel>? hotels = [];
            var response = await _apiService
                .GetAsync<ApiResponse<IEnumerable<HotelCardViewModel>>>(_hotelEndpoint);
            
            if (response is null)
                return View(hotels);
            
            hotels = response.Data;
            return View(hotels);    
        }
        catch (Exception)
        {
            ViewData["HotelsError"] = "Error fetching hotels.";
            return View();
        }
    }

    [HttpPost]
    public IActionResult Index(int hotelId)
    {
        HttpContext.Session.SetInt32("HotelId", hotelId);
        return RedirectToAction(nameof(Details));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details()
    {
        try
        {
            var hotelId = HttpContext.Session.GetInt32("HotelId");

            var hotelDetails = new HotelDetailsViewModel();
            var hotelResponse = await _apiService
                .GetAsync<ApiResponse<HotelCardViewModel>>($"{_hotelEndpoint}/{hotelId}");
            
            var amenityResponse = await _apiService
                .GetAsync<ApiResponse<IEnumerable<AmenityViewModel>>>($"{_hotelEndpoint}/{hotelId}/amenities");

            var roomResponse = await _apiService
                .GetAsync<ApiResponse<IEnumerable<RoomPriceViewModel>>>($"{_hotelEndpoint}/{hotelId}/available-rooms");
            
            if (hotelResponse is null || amenityResponse is null || roomResponse is null)
                return View(hotelDetails);
            
            hotelDetails.HotelId = hotelResponse!.Data!.HotelId;
            hotelDetails.Name = hotelResponse.Data.Name;
            hotelDetails.Description = hotelResponse.Data.Description;
            hotelDetails.Location = hotelResponse.Data.Location;
            hotelDetails.Amenities = amenityResponse!.Data!.ToList();
            hotelDetails.Rooms = roomResponse!.Data!.ToList();

            return View(hotelDetails);    
        }
        catch (Exception ex)
        {
            ViewData["HotelDetailsError"] = ex.Message;
            return View();
        }
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Details(string roomType, int roomId)
    {
        HttpContext.Session.SetString("RoomType", roomType);
        HttpContext.Session.SetInt32("RoomId", roomId);

        return RedirectToAction("Create", "Reservation");
    }
}
