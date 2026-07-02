using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.TimeClock.Commands.ClockOut;

/// <summary>Le soignant connecté pointe son départ (clôt son pointage ouvert). Ne renvoie rien.</summary>
public record ClockOutCommand() : ICommand<Unit>;
