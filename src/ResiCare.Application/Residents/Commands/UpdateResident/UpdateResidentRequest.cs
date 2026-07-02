using ResiCare.Domain.Enums;

namespace ResiCare.Application.Residents.Commands.UpdateResident;

/// <summary>Corps JSON du PUT, SANS l'Id (qui vient de l'URL) ni les dates (non modifiables).</summary>
public record UpdateResidentRequest(
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
    Guid ReferentCaregiverId);
