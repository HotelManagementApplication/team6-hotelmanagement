namespace HotelManagement.MVC.ViewModels;

public class ReviewDetailsViewModel
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateOnly? ReviewDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string GuestPhoneNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
}