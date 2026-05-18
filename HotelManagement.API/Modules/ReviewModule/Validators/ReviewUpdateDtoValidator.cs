using HotelManagement.API.Modules.ReviewModule.DTOs;

using FluentValidation;

namespace HotelManagement.API.Modules.ReviewModule.Validators;

public class ReviewUpdateDtoValidator : AbstractValidator<ReviewUpdateDto>
{
    public ReviewUpdateDtoValidator()
    {
        RuleFor(review => review.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(review => review.Comment)
            .MaximumLength(1000)
            .WithMessage("Review comment cannot exceed 1000 characters.");
    }
}
