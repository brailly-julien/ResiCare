using ResiCare.Domain.Enums;

namespace ResiCare.Application.Residents.Queries.GetResidents;

/// <summary>
/// Vue "résumé" d'un résident pour la liste. La dépendance est résumée par son niveau
/// global (le plus dépendant des activités). On expose aussi le risque de chute pour un badge.
/// </summary>
public record ResidentSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    string RoomNumber,
    AutonomyLevel OverallDependency,
    RiskLevel FallRisk,
    bool IsArchived);
