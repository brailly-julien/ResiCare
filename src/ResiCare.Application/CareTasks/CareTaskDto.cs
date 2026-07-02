using ResiCare.Domain.Enums;

namespace ResiCare.Application.CareTasks;

/// <summary>Vue d'une tâche de soin (lecture). Partagée par la liste et le tableau de bord.</summary>
public record CareTaskDto(
    Guid Id,
    string Label,
    DateOnly ScheduledDate,
    CareTaskStatus Status,
    Guid? CompletedByCaregiverId,
    DateTime? CompletedAt);
