using ResiCare.Application.Common.Messaging;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Prescriptions.Commands.PrescribeMedication;

/// <summary>Le responsable prescrit un médicament à un résident. Renvoie l'Id de la prescription.</summary>
public record PrescribeMedicationCommand(
    Guid ResidentId,
    string MedicationName,
    string Dosage,
    string Posology,
    MedicationRoute Route,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Instructions) : ICommand<Guid>;
