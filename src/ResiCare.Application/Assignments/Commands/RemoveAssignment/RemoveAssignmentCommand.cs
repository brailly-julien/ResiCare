using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Assignments.Commands.RemoveAssignment;

/// <summary>Le responsable retire une affectation. Ne renvoie rien (Unit).</summary>
public record RemoveAssignmentCommand(Guid Id) : ICommand<Unit>;
