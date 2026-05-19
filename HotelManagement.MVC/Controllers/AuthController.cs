using System.Security.Claims;
using HotelManagement.Common.DTOs;
using HotelManagement.MVC.Services;
using HotelManagement.MVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.MVC.Controllers;

public class AuthController(ApiService apiService) : Controller
{
    private readonly ApiService _apiService = apiService;
    private readonly string _endpoint = "auth";

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        try
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService
                .PostAsync<ApiResponse<Dictionary<string, string>>>($"{_endpoint}/login", model);
            
            if (response is null)
                throw new NullReferenceException();

            var token = response.Data!["token"];
            var role = response.Data!["role"];

            System.Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAa");
            System.Console.WriteLine(role);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Role, role!)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);
            
            var authProperties = new AuthenticationProperties { IsPersistent = true };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            HttpContext.Session.SetString("JwtToken", token);
            Response.Cookies.Append("JwtToken", token);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Hotels");
        }
        catch (NullReferenceException)
        {
            ViewData["LoginError"] = "Something went wrong while attempting to log in.";
            return View(model);
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }

    [AllowAnonymous]
    public IActionResult Register() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        try
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<ApiResponse<Dictionary<string, string>>>(
                $"{_endpoint}/register", new 
                {
                    model.Email,
                    model.Password,
                    Role = "User"
                });

            if (response is null)
                throw new NullReferenceException();

            var loginModel = new LoginViewModel
            {
                Email = model.Email,
                Password = model.Password
            };
            return RedirectToAction(nameof(Login));
        }
        catch (NullReferenceException)
        {
            ViewData["RegisterError"] = "Something went wrong while trying to register user.";
            return View(model);
        }
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();
            foreach (var cookie in Request.Cookies.Keys)
                Response.Cookies.Delete(cookie);

            return RedirectToAction("Index", "Hotels");    
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message);
        };
    }
}
