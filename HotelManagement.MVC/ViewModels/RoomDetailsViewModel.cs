namespace HotelManagement.MVC.ViewModels;

public class RoomViewModel
{
    public int RoomId { get; set; }
    public int RoomNumber { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}

public class RoomDetailsViewModel
{
    public int RoomId { get; set; }
    public int RoomNumber { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public IEnumerable<AmenityViewModel> Amenities { get; set; } = [];
}
