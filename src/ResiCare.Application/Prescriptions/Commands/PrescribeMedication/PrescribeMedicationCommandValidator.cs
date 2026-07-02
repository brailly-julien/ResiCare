using FluentValidation;

namespace ResiCare.Application.Prescriptions.Commands.PrescribeMedication;

public sealed class PrescribeMedicationCommandValidator : AbstractValidator<PrescribeMedicationCommand>
{
    public PrescribeMedicationCommandValidator()
    {
        RuleFor(x => x.ResidentId).NotEmpty();
        RuleFor(x => x.MedicationName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dosage).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Posology).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Route).IsInEnum();
        RuleFor(x => x.Instructions).MaximumLength(1000);
    }
}
