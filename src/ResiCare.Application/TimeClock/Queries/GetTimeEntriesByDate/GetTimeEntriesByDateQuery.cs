using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.TimeClock.Queries.GetTimeEntriesByDate;

/// <summary>Tous les pointages d'une date, avec le nom du soignant — vue « présences » (responsable).</summary>
public record GetTimeEntriesByDateQuery(DateOnly Date) : IQuery<IReadOnlyList<CaregiverTimeEntryDto>>;
