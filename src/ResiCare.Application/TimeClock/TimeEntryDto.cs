namespace ResiCare.Application.TimeClock;

/// <summary>Vue d'un pointage (lecture). <c>DurationMinutes</c> est null tant qu'il est ouvert.</summary>
public record TimeEntryDto(
    Guid Id,
    Guid CaregiverId,
    DateTime ClockInAt,
    DateTime? ClockOutAt,
    int? DurationMinutes);
