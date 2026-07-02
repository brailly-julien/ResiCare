using ResiCare.Application.Common.Messaging;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Assignments.Commands.AssignCaregiver;

/// <summary>Le responsable affecte un soignant à un résident pour une date et un poste. Renvoie l'Id.</summary>
public record AssignCaregiverCommand(Guid CaregiverId, Guid ResidentId, DateOnly Date, Shift Shift) : ICommand<Guid>;
