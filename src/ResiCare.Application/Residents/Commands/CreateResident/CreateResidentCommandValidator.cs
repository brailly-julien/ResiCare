using FluentValidation;

namespace ResiCare.Application.Residents.Commands.CreateResident;

public sealed class CreateResidentCommandValidator : AbstractValidator<CreateResidentCommand>
{
    public CreateResidentCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.RoomNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ReferentCaregiverId).NotEmpty();

        RuleFor(x => x.Eating).IsInEnum();
        RuleFor(x => x.Elimination).IsInEnum();
        RuleFor(x => x.Mobility).IsInEnum();
        RuleFor(x => x.Dressing).IsInEnum();
        RuleFor(x => x.Hygiene).IsInEnum();

        RuleFor(x => x.FallRisk).IsInEnum();
        RuleFor(x => x.PressureSoreRisk).IsInEnum();
        RuleFor(x => x.MalnutritionRisk).IsInEnum();

        RuleFor(x => x.AttendingPhysician).MaximumLength(200);
        RuleFor(x => x.EmergencyContactName).MaximumLength(200);
        RuleFor(x => x.EmergencyContactPhone).MaximumLength(30);
        RuleFor(x => x.Occupation).MaximumLength(200);
        RuleFor(x => x.Interests).MaximumLength(1000);
        RuleFor(x => x.Family).MaximumLength(1000);

        RuleFor(x => x.AdmissionDate)
            .GreaterThanOrEqualTo(x => x.BirthDate)
            .WithMessage("La date d'admission ne peut pas précéder la date de naissance.");
    }
}
