namespace ResiCare.Application.CareTasks.Commands.CreateCareTask;

/// <summary>Corps JSON du POST (le ResidentId vient de l'URL).</summary>
public record CreateCareTaskRequest(string Label, DateOnly ScheduledDate);
