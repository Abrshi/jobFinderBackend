using FluentValidation;
using jobFinder.Application.DTOs.Auth;

namespace jobFinder.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Please provide a valid email address.")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
    }
}