using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.Caregivers.Queries.GetCaregivers;

public sealed class GetCaregiversQueryHandler
    : IQueryHandler<GetCaregiversQuery, IReadOnlyList<CaregiverDto>>
{
    private readonly IApplicationDbContext _db;

    public GetCaregiversQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<CaregiverDto>> Handle(
        GetCaregiversQuery query, CancellationToken cancellationToken)
    {
        return await _db.Caregivers.AsNoTracking()
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new CaregiverDto(c.Id, c.FirstName, c.LastName, c.Role))
            .ToListAsync(cancellationToken);
    }
}
