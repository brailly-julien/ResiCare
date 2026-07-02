using FluentValidation;

namespace ResiCare.Application.Observations.Commands.AddObservation;

public sealed class AddObservationCommandValidator : AbstractValidator<AddObservationCommand>
{
    public AddObservationCommandValidator()
    {
        RuleFor(x => x.ResidentId).NotEmpty();
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}
