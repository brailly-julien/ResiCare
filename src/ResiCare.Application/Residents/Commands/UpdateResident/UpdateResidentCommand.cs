using ResiCare.Application.Common.Messaging;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Residents.Commands.UpdateResident;

/// <summary>Modifie un résident (les dates ne sont pas modifiables). Ne renvoie rien (Unit).</summary>
public record UpdateResidentCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string RoomNumber,
    AutonomyLevel Eating,
    AutonomyLevel Elimination,
    AutonomyLevel Mobility,
    AutonomyLevel Dressing,
    AutonomyLevel Hygiene,
    RiskLevel FallRisk,
    RiskLevel PressureSoreRisk,
    RiskLevel MalnutritionRisk,
    string? AttendingPhysician,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Occupation,
    string? Interests,
    string? Family,
    Guid ReferentCaregiverId) : ICommand<Unit>;
