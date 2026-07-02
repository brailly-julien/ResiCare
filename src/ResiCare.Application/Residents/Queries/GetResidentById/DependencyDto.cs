using ResiCare.Domain.Enums;

namespace ResiCare.Application.Residents.Queries.GetResidentById;

/// <summary>Détail du profil de dépendance par activité (pour le tableau de bord).</summary>
public record DependencyDto(
    AutonomyLevel Eating,
    AutonomyLevel Elimination,
    AutonomyLevel Mobility,
    AutonomyLevel Dressing,
    AutonomyLevel Hygiene);
