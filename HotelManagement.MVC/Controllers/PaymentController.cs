using System.Text.Json;
using HotelManagement.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.MVC.Controllers;

public class PaymentController : Controller
{
    public IActionResult Index()
    {
        var json = (TempData["ReservationDetails"] as string) ?? string.Empty;
        var model = JsonSerializer.Deserialize<ReservationViewModel>(json);
        return View(model);
    }
}