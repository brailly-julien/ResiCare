using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Common.Security;

namespace ResiCare.Application.Assignments.Queries.GetMyAssignments;

public sealed class GetMyAssignmentsQueryHandler
    : IQueryHandler<GetMyAssignmentsQuery, IReadOnlyList<AssignmentResidentDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetMyAssignmentsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<AssignmentResidentDto>> Handle(
        GetMyAssignmentsQuery query, CancellationToken cancellationToken)
    {
        var caregiverId = _currentUser.Id ?? throw new UnauthorizedException("Authentification requise.");

        return await (
            from a in _db.Assignments.AsNoTracking()
            join r in _db.Residents.AsNoTracking() on a.ResidentId equals r.Id
            where a.CaregiverId == caregiverId && a.Date == query.Date
            orderby a.Shift, r.RoomNumber
            select new AssignmentResidentDto(
                a.Id, a.ResidentId, r.FirstName, r.LastName, r.RoomNumber, a.Date, a.Shift))
            .ToListAsync(cancellationToken);
    }
}
