namespace HotelManagement.API.Modules.ReservationModule.DTOs;

public class CreateReservationDto
{
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string GuestPhoneNumber { get; set; } = string.Empty;

    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }

    public int RoomId { get; set; }
    public int HotelId { get; set; }
}