using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.CareTasks.Commands.CompleteCareTask;

/// <summary>
/// Marque une tâche comme réalisée. Ne renvoie rien (Unit).
/// Le soignant qui valide est déduit du jeton JWT (et non envoyé par le client).
/// </summary>
public record CompleteCareTaskCommand(Guid TaskId) : ICommand<Unit>;
