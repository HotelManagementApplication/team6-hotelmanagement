using HotelManagement.API.Modules.AmenityModule.DTOs;

using FluentValidation;

namespace HotelManagement.API.Modules.AmenityModule.Validators;

public class AmenityCreateDtoValidator : AbstractValidator<AmenityCreateDto>
{
    public AmenityCreateDtoValidator()
    {
        RuleFor(amenity => amenity.Name)
            .NotEmpty()
            .WithMessage("Amenity name is required.")
            .MaximumLength(100)
            .WithMessage("Amenity name cannot exceed 100 characters.");

        RuleFor(amenity => amenity.Description)
            .MaximumLength(500)
            .WithMessage("Amenity description cannot exceed 500 characters.");
    }
}
