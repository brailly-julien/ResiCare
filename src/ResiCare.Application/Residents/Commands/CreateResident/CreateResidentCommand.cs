using ResiCare.Application.Common.Messaging;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Residents.Commands.CreateResident;

/// <summary>Commande de création d'un résident. Sert aussi de DTO d'entrée de l'API.</summary>
public record CreateResidentCommand(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    DateOnly AdmissionDate,
    string RoomNumber,
    // Profil de dépendance (Henderson)
    AutonomyLevel Eating,
    AutonomyLevel Elimination,
    AutonomyLevel Mobility,
    AutonomyLevel Dressing,
    AutonomyLevel Hygiene,
    // Risques (niveau : None / Low / Moderate / High)
    RiskLevel FallRisk,
    RiskLevel PressureSoreRisk,
    RiskLevel MalnutritionRisk,
    // Infos administratives + bio (optionnelles)
    string? AttendingPhysician,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Occupation,
    string? Interests,
    string? Family,
    Guid ReferentCaregiverId) : ICommand<Guid>;
