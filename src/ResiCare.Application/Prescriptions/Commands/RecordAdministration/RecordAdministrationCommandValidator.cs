using FluentValidation;

namespace ResiCare.Application.Prescriptions.Commands.RecordAdministration;

public sealed class RecordAdministrationCommandValidator : AbstractValidator<RecordAdministrationCommand>
{
    public RecordAdministrationCommandValidator()
    {
        RuleFor(x => x.PrescriptionId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
