using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.MVC.Controllers;

public class ErrorController : Controller
{
    public IActionResult Index()
    {
        TempData["PageError"] = "Invalid URL Access: User tried to access a url that does not exists.";
        return RedirectToAction("Index", "Home");
    }
}
