using FluentValidation;
using jobFinderBackend.Application.Profile.DTOs;

namespace jobFinderBackend.Application.Profile.Validators;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Education)
            .NotNull()
            .WithMessage("Education cannot be null.");

        RuleFor(x => x.Experience)
            .NotNull()
            .WithMessage("Experience cannot be null.");

        RuleForEach(x => x.Education).ChildRules(education =>
        {
            education.RuleFor(x => x.Id)
                .GreaterThan(0)
                .When(x => x.Id.HasValue)
                .WithMessage("Education ID must be greater than zero.");

            education.RuleFor(x => x.Institution)
                .NotEmpty()
                .WithMessage("Institution is required.")
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .WithMessage("Institution is required.")
                .MaximumLength(255);

            education.RuleFor(x => x.Degree)
                .NotEmpty()
                .WithMessage("Degree is required.")
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .WithMessage("Degree is required.")
                .MaximumLength(255);

            education.RuleFor(x => x.FieldOfStudy)
                .MaximumLength(255);

            education.RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("End date cannot be before start date.");
        });

        RuleForEach(x => x.Experience).ChildRules(experience =>
        {
            experience.RuleFor(x => x.Id)
                .GreaterThan(0)
                .When(x => x.Id.HasValue)
                .WithMessage("Experience ID must be greater than zero.");

            experience.RuleFor(x => x.Company)
                .MaximumLength(255);

            experience.RuleFor(x => x.Position)
                .NotEmpty()
                .WithMessage("Position is required.")
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .WithMessage("Position is required.")
                .MaximumLength(255);

            experience.RuleFor(x => x.Description)
                .MaximumLength(4000);

            experience.RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.EndDate.HasValue)
                .WithMessage("End date cannot be before start date.");

            experience.RuleFor(x => x.EndDate)
                .Null()
                .When(x => x.IsCurrent)
                .WithMessage("Current experience cannot have an end date.");
        });
    }
}