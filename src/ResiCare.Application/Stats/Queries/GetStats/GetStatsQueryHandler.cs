using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Stats.Queries.GetStats;

public sealed class GetStatsQueryHandler : IQueryHandler<GetStatsQuery, StatsDto>
{
    private readonly IApplicationDbContext _db;

    public GetStatsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<StatsDto> Handle(GetStatsQuery query, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var startOfDay = DateTime.UtcNow.Date;
        var endOfDay = startOfDay.AddDays(1);

        var activeResidents = await _db.Residents
            .CountAsync(r => !r.IsArchived, cancellationToken);

        var observationsToday = await _db.Observations
            .CountAsync(o => o.CreatedAt >= startOfDay && o.CreatedAt < endOfDay, cancellationToken);

        var tasksToday = await _db.CareTasks
            .CountAsync(t => t.ScheduledDate == today, cancellationToken);

        var tasksDoneToday = await _db.CareTasks
            .CountAsync(t => t.ScheduledDate == today && t.Status == CareTaskStatus.Done, cancellationToken);

        return new StatsDto(activeResidents, observationsToday, tasksToday, tasksDoneToday);
    }
}
