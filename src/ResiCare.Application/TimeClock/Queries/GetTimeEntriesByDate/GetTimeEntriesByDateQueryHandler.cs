using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.TimeClock.Queries.GetTimeEntriesByDate;

public sealed class GetTimeEntriesByDateQueryHandler
    : IQueryHandler<GetTimeEntriesByDateQuery, IReadOnlyList<CaregiverTimeEntryDto>>
{
    private readonly IApplicationDbContext _db;

    public GetTimeEntriesByDateQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<CaregiverTimeEntryDto>> Handle(
        GetTimeEntriesByDateQuery query, CancellationToken cancellationToken)
    {
        var start = query.Date.ToDateTime(TimeOnly.MinValue);
        var end = start.AddDays(1);

        // Jointure pointage <-> soignant (FK par Id, pas de navigation) pour récupérer le nom.
        var rows = await (
            from e in _db.TimeEntries.AsNoTracking()
            join c in _db.Caregivers.AsNoTracking() on e.CaregiverId equals c.Id
            where e.ClockInAt >= start && e.ClockInAt < end
            orderby c.LastName, e.ClockInAt
            select new { Entry = e, c.FirstName, c.LastName })
            .ToListAsync(cancellationToken);

        return rows.Select(r => new CaregiverTimeEntryDto(
                r.Entry.Id, r.Entry.CaregiverId, r.FirstName, r.LastName,
                r.Entry.ClockInAt, r.Entry.ClockOutAt,
                r.Entry.Duration is { } d ? (int)d.TotalMinutes : null))
            .ToList();
    }
}
