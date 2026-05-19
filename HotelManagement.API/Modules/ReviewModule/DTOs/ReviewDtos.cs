using System.ComponentModel.DataAnnotations;

namespace HotelManagement.API.Modules.ReviewModule.DTOs;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public int ReservationId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateOnly? ReviewDate { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class ReviewCreateDto
{
    [Required]
    public int ReservationId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
}

public class ReviewUpdateDto
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
}

public class ReviewDetailsDto
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