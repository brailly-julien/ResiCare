using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Common.Security;
using ResiCare.Domain.Entities;

namespace ResiCare.Application.TimeClock.Queries.GetMyTimeEntries;

public sealed class GetMyTimeEntriesQueryHandler
    : IQueryHandler<GetMyTimeEntriesQuery, IReadOnlyList<TimeEntryDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetMyTimeEntriesQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<TimeEntryDto>> Handle(
        GetMyTimeEntriesQuery query, CancellationToken cancellationToken)
    {
        var caregiverId = _currentUser.Id ?? throw new UnauthorizedException("Authentification requise.");

        // Bornes du jour : filtre traduisible par tous les providers (vs DateOnly.FromDateTime).
        var start = query.Date.ToDateTime(TimeOnly.MinValue);
        var end = start.AddDays(1);

        var entries = await _db.TimeEntries.AsNoTracking()
            .Where(e => e.CaregiverId == caregiverId && e.ClockInAt >= start && e.ClockInAt < end)
            .OrderByDescending(e => e.ClockInAt)
            .ToListAsync(cancellationToken);

        // La durée (propriété calculée, non mappée) est dérivée en mémoire.
        return entries.Select(ToDto).ToList();
    }

    private static TimeEntryDto ToDto(TimeEntry e) =>
        new(e.Id, e.CaregiverId, e.ClockInAt, e.ClockOutAt,
            e.Duration is { } d ? (int)d.TotalMinutes : null);
}
