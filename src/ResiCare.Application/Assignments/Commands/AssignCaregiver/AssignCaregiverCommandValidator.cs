using FluentValidation;

namespace ResiCare.Application.Assignments.Commands.AssignCaregiver;

public sealed class AssignCaregiverCommandValidator : AbstractValidator<AssignCaregiverCommand>
{
    public AssignCaregiverCommandValidator()
    {
        RuleFor(x => x.CaregiverId).NotEmpty();
        RuleFor(x => x.ResidentId).NotEmpty();
        RuleFor(x => x.Shift).IsInEnum();
    }
}
