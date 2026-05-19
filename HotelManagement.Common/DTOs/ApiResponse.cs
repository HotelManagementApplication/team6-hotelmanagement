namespace HotelManagement.Common.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; } = null;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; } = null;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(int statusCode, string? message, T? data) =>
        new() { Success = true, StatusCode = statusCode, Message = message, Data = data }; 
    
    public static ApiResponse<T> Fail(int statusCode, string? message, List<string>? errors = null) =>
        new() { Success = false, StatusCode = statusCode, Message = message, Errors = errors};
}
