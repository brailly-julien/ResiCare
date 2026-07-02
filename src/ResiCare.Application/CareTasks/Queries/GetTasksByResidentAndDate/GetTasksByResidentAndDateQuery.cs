using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.CareTasks.Queries.GetTasksByResidentAndDate;

/// <summary>Tâches d'un résident pour une date donnée (par défaut : aujourd'hui).</summary>
public record GetTasksByResidentAndDateQuery(Guid ResidentId, DateOnly? Date)
    : IQuery<IReadOnlyList<CareTaskDto>>;
