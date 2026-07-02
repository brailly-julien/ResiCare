using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.Assignments.Queries.GetAssignmentsByDate;

public sealed class GetAssignmentsByDateQueryHandler
    : IQueryHandler<GetAssignmentsByDateQuery, IReadOnlyList<AssignmentDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAssignmentsByDateQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<AssignmentDetailDto>> Handle(
        GetAssignmentsByDateQuery query, CancellationToken cancellationToken)
    {
        // Jointures par Id (pas de navigation) pour récupérer les noms soignant + résident.
        return await (
            from a in _db.Assignments.AsNoTracking()
            join c in _db.Caregivers.AsNoTracking() on a.CaregiverId equals c.Id
            join r in _db.Residents.AsNoTracking() on a.ResidentId equals r.Id
            where a.Date == query.Date
            orderby a.Shift, r.LastName
            select new AssignmentDetailDto(
                a.Id,
                a.CaregiverId, c.FirstName, c.LastName,
                a.ResidentId, r.FirstName, r.LastName, r.RoomNumber,
                a.Date, a.Shift))
            .ToListAsync(cancellationToken);
    }
}
