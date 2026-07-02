using ResiCare.Domain.Enums;

namespace ResiCare.Application.Prescriptions;

/// <summary>Vue d'une prescription (lecture).</summary>
public record PrescriptionDto(
    Guid Id,
    Guid ResidentId,
    string MedicationName,
    string Dosage,
    string Posology,
    MedicationRoute Route,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Instructions,
    bool IsDiscontinued);
