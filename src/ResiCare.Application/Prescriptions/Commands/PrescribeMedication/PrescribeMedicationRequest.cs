using ResiCare.Domain.Enums;

namespace ResiCare.Application.Prescriptions.Commands.PrescribeMedication;

/// <summary>Corps JSON du POST (le ResidentId vient de l'URL).</summary>
public record PrescribeMedicationRequest(
    string MedicationName,
    string Dosage,
    string Posology,
    MedicationRoute Route,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Instructions);
