namespace ResiCare.Application.TimeClock;

/// <summary>Pointage enrichi du nom du soignant — pour la vue « présences » du responsable.</summary>
public record CaregiverTimeEntryDto(
    Guid Id,
    Guid CaregiverId,
    string CaregiverFirstName,
    string CaregiverLastName,
    DateTime ClockInAt,
    DateTime? ClockOutAt,
    int? DurationMinutes);
