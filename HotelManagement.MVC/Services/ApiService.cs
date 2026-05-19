using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HotelManagement.MVC.Services;

public class ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private static readonly JsonSerializerOptions _jsonOptions = 
        new() { PropertyNameCaseInsensitive = true };

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("FoodDelivaryAPI");
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
        {
            token = _httpContextAccessor.HttpContext?.Request.Cookies["JwtToken"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var client = CreateClient();
        var response = await client.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode) return default;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object body)
    {
        var client = CreateClient();
        var content = new StringContent(
            JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endpoint, content);

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }
}
