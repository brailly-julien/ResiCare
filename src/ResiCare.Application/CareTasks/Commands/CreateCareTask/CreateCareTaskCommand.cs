using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.CareTasks.Commands.CreateCareTask;

/// <summary>Planifie une tâche de soin pour un résident. Renvoie l'Id de la tâche créée.</summary>
public record CreateCareTaskCommand(
    Guid ResidentId,
    string Label,
    DateOnly ScheduledDate) : ICommand<Guid>;
