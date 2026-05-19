namespace HotelManagement.MVC.ViewModels;

public class HotelDetailsViewModel
{
    public int HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<AmenityViewModel> Amenities { get; set; } = new();
    public List<RoomPriceViewModel> Rooms { get; set; } = new();
}

public class RoomPriceViewModel
{
    public int RoomId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxOccupancy { get; set; }
}

public class AmenityViewModel
{
    public int AmenityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
