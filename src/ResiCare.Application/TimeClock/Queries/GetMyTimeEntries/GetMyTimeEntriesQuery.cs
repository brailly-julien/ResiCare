using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.TimeClock.Queries.GetMyTimeEntries;

/// <summary>Les pointages du soignant connecté pour une date donnée (triés du plus récent).</summary>
public record GetMyTimeEntriesQuery(DateOnly Date) : IQuery<IReadOnlyList<TimeEntryDto>>;
