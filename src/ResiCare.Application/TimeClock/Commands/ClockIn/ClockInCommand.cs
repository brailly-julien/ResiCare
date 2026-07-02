using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.TimeClock.Commands.ClockIn;

/// <summary>Le soignant connecté pointe son arrivée. Renvoie l'Id du pointage ouvert.</summary>
public record ClockInCommand() : ICommand<Guid>;
