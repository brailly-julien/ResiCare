using ResiCare.Application.CareTasks;
using ResiCare.Application.Observations;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Residents.Queries.GetResidentById;

/// <summary>Vue "tableau de bord" : fiche complète + observations récentes + tâches du jour.</summary>
public record ResidentDashboardDto(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    DateOnly AdmissionDate,
    string RoomNumber,
    DependencyDto Dependency,
    Guid ReferentCaregiverId,
    bool IsArchived,
    RiskLevel FallRisk,
    RiskLevel PressureSoreRisk,
    RiskLevel MalnutritionRisk,
    string? AttendingPhysician,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Occupation,
    string? Interests,
    string? Family,
    IReadOnlyList<ObservationDto> RecentObservations,
    IReadOnlyList<CareTaskDto> TodayTasks);
