namespace ResiCare.Application.Stats;

/// <summary>Statistiques simples du jour pour le tableau de bord d'accueil.</summary>
public record StatsDto(
    int ActiveResidents,
    int ObservationsToday,
    int TasksToday,
    int TasksDoneToday);
