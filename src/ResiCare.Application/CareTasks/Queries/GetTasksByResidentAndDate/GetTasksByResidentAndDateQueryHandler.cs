using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.CareTasks.Queries.GetTasksByResidentAndDate;

public sealed class GetTasksByResidentAndDateQueryHandler
    : IQueryHandler<GetTasksByResidentAndDateQuery, IReadOnlyList<CareTaskDto>>
{
    private readonly IApplicationDbContext _db;

    public GetTasksByResidentAndDateQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<CareTaskDto>> Handle(
        GetTasksByResidentAndDateQuery query, CancellationToken cancellationToken)
    {
        // Si aucune date n'est fournie, on prend aujourd'hui (les "tâches du jour").
        var date = query.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        return await _db.CareTasks.AsNoTracking()
            .Where(t => t.ResidentId == query.ResidentId && t.ScheduledDate == date)
            .OrderBy(t => t.Label)
            .Select(t => new CareTaskDto(
                t.Id, t.Label, t.ScheduledDate, t.Status, t.CompletedByCaregiverId, t.CompletedAt))
            .ToListAsync(cancellationToken);
    }
}
