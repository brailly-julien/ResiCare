using ResiCare.Domain.Enums;

namespace ResiCare.Application.Prescriptions;

/// <summary>Vue d'une administration (feuille de soins), enrichie du nom du médicament.</summary>
public record AdministrationDto(
    Guid Id,
    Guid PrescriptionId,
    string MedicationName,
    Guid CaregiverId,
    DateTime AdministeredAt,
    AdministrationStatus Status,
    string? Notes);
