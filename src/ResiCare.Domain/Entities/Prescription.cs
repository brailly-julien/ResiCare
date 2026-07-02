using ResiCare.Domain.Common;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Entities;

/// <summary>
/// Une prescription médicale pour un résident : médicament, dose, posologie (fréquence), voie
/// d'administration et période. Saisie par le responsable. Peut être ARRÊTÉE — on conserve
/// l'historique (soft) plutôt que de supprimer.
/// </summary>
public class Prescription
{
    public Guid Id { get; private set; }
    public Guid ResidentId { get; private set; }
    public string MedicationName { get; private set; } = null!;
    public string Dosage { get; private set; } = null!;
    public string Posology { get; private set; } = null!;
    public MedicationRoute Route { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? Instructions { get; private set; }
    public bool IsDiscontinued { get; private set; }

    // Constructeur réservé à EF Core.
    private Prescription()
    {
    }

    public Prescription(
        Guid residentId, string medicationName, string dosage, string posology,
        MedicationRoute route, DateOnly startDate, DateOnly? endDate, string? instructions)
    {
        Guard.AgainstEmptyGuid(residentId, "Le résident");
        MedicationName = Guard.AgainstNullOrWhiteSpace(medicationName, "Le médicament");
        Dosage = Guard.AgainstNullOrWhiteSpace(dosage, "La dose");
        Posology = Guard.AgainstNullOrWhiteSpace(posology, "La posologie");
        Guard.AgainstUndefinedEnum(route, "La voie d'administration");

        if (endDate is not null && endDate < startDate)
            throw new DomainException("La date de fin ne peut pas précéder la date de début.");

        Id = Guid.CreateVersion7();
        ResidentId = residentId;
        Route = route;
        StartDate = startDate;
        EndDate = endDate;
        Instructions = string.IsNullOrWhiteSpace(instructions) ? null : instructions.Trim();
        IsDiscontinued = false;
    }

    public bool IsActive => !IsDiscontinued;

    /// <summary>Arrête la prescription. Règle métier : on ne l'arrête pas deux fois.</summary>
    public void Discontinue()
    {
        if (IsDiscontinued)
            throw new DomainException("Cette prescription est déjà arrêtée.");

        IsDiscontinued = true;
    }
}
