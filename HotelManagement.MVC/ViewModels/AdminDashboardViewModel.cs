namespace HotelManagement.MVC.ViewModels;

public class AdminDashboardViewModel
{
    public IEnumerable<AmenityViewModel> Amenities { get; set; } = [];
    public IEnumerable<HotelCardViewModel> Hotels { get; set; } = [];
    public IEnumerable<RoomViewModel> Rooms { get; set; } = [];
    public IEnumerable<RoomTypeViewModel> RoomTypes { get; set; } = [];
    public IEnumerable<ReviewViewModel> Reviews { get; set; } = [];
    public IEnumerable<ReservationViewModel> Reservations { get; set; } = [];
    public IEnumerable<PaymentDetailViewModel> Payments { get; set; } = [];
}

public class RoomTypeViewModel
{
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? MaxOccupancy { get; set; }
    public decimal? PricePerNight { get; set; }
}

public class ReviewViewModel
{
    public int ReviewId { get; set; }
    public int ReservationId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateOnly? ReviewDate { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class PaymentDetailViewModel
{
    public int ReservationId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly PaymentDate { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
}