using System.Text.Json;
using HotelManagement.Common.DTOs;
using HotelManagement.MVC.Services;
using HotelManagement.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.MVC.Controllers;

public class ReservationController(ApiService apiService) : Controller
{
    private readonly ApiService _apiService = apiService;
    private readonly string _endpoint = "reservation";

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(ReservationViewModel model)
    {
        if (!ModelState.IsValid)
            return View();
        
        TempData["ReservationDetails"] = JsonSerializer.Serialize(model);
        TempData.Keep("ReservationDetails");
        return RedirectToAction(nameof(Confirm));
    }

    public IActionResult Confirm()
    {
        var data = TempData["ReservationDetails"] as string;
        if (data is null)
            return RedirectToAction(nameof(Create));
            
        var reservationDetails = JsonSerializer.Deserialize<ReservationViewModel>(data!);
        return View(reservationDetails);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Confirm(ReservationViewModel model)
    {
        var response = await _apiService.PostAsync<ApiResponse<string>>(_endpoint, new
        {
            model.GuestName,
            model.GuestEmail,
            model.GuestPhoneNumber,
            model.CheckInDate,
            model.CheckOutDate,
            model.RoomId
        });

        if (response is null)
        {
            TempData["ReservationError"] = "Internal Server Error";
            return View("Create", model);
        }

        if (!response.Success)
        {
            TempData["ReservationError"] = response.Message;
            return View("Create", model);
        }

        TempData["ReservationDetails"] = JsonSerializer.Serialize(
            model, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return RedirectToAction("Index", "Payment");
    }
}
