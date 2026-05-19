namespace HotelManagement.MVC.ViewModels;

using System.ComponentModel.DataAnnotations;

public class ReservationViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Guest name is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Guest name must be between 2 and 100 characters.")]
    [RegularExpression(
        @"^[a-zA-Z\s]+$",
        ErrorMessage = "Guest name can contain only letters and spaces."
    )]
    public string GuestName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(100)]
    public string GuestEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    [RegularExpression(
        @"^[0-9]{10,15}$",
        ErrorMessage = "Phone number must contain 10 to 15 digits."
    )]
    public string GuestPhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Room type is required.")]
    public string RoomType { get; set; } = string.Empty;

    public int RoomId { get; set; }

    [Required(ErrorMessage = "Check-in date is required.")]
    [DataType(DataType.Date)]
    public DateOnly CheckInDate { get; set; }

    [Required(ErrorMessage = "Check-out date is required.")]
    [DataType(DataType.Date)]
    public DateOnly CheckOutDate { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult(
                "Check-out date must be after check-in date.",
                new[] { nameof(CheckOutDate) });
        }

        if (CheckInDate < DateOnly.FromDateTime(DateTime.Today))
        {
            yield return new ValidationResult(
                "Check-in date cannot be in the past.",
                new[] { nameof(CheckInDate) });
        }
    }
}
public class BookingViewModel
{
    // ignore reservation id, room type id, hotel id, price per night
    public int ReservationId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string GuestPhoneNumber { get; set; } = string.Empty;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public DateOnly BookingDate { get; set; }
    public int RoomNumber { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public List<AmenityViewModel> Amenities { get; set; } = [];
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string HotelLocation { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}
