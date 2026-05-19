using FluentValidation;
using HotelManagement.API.Modules.AuthModule.DTOs;

namespace HotelManagement.API.Modules.AuthModule.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty()
            .WithMessage("Email should not be empty.")
            .EmailAddress()
            .Matches(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$")
            .WithMessage("Invalid meail address");

        RuleFor(user => user.Password)
            .NotEmpty()
            .WithMessage("Password should not be empty.")
            .MinimumLength(8)
            .WithMessage("Password should be at least 8 characters long.")
            .MaximumLength(30)
            .WithMessage("Password should be no more than 30 characters long.");
    }
}
