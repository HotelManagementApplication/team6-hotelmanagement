using System.ComponentModel.DataAnnotations;

namespace HotelManagement.API.Modules.RoomModule.DTOs;

// ── Room DTOs ──────────────────────────────────────────────────────────────

public class RoomDto
{
    public int RoomId { get; set; }
    public int? RoomNumber { get; set; }
    public int? RoomTypeId { get; set; }
    public string? RoomTypeName { get; set; }
    public bool? IsAvailable { get; set; }
}

public class RoomWithPriceDto
{
    public int RoomId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxOccupancy { get; set; }
}

public class RoomCreateDto
{
    [Required]
    [Range(1, 9999)]
    public int RoomNumber { get; set; }

    [Required]
    public int RoomTypeId { get; set; }

    public bool IsAvailable { get; set; } = true;
}

public class RoomUpdateDto : RoomCreateDto;

public class RoomAvailabilityUpdateDto
{
    [Required]
    public bool IsAvailable { get; set; }
}

// ── RoomType DTOs ──────────────────────────────────────────────────────────

public class RoomTypeDto
{
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? MaxOccupancy { get; set; }
    public decimal? PricePerNight { get; set; }
}

public class RoomTypeCreateDto
{
    [Required]
    [MaxLength(255)]
    public string TypeName { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(1, 20)]
    public int? MaxOccupancy { get; set; }

    [Range(0.01, 100000)]
    public decimal? PricePerNight { get; set; }
}

public class RoomTypeUpdateDto : RoomTypeCreateDto;

public class RoomTypePriceUpdateDto
{
    [Required]
    [Range(0.01, 100000)]
    public decimal PricePerNight { get; set; }
}
