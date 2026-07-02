using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.Residents.Queries.GetResidents;

public sealed class GetResidentsQueryHandler
    : IQueryHandler<GetResidentsQuery, IReadOnlyList<ResidentSummaryDto>>
{
    private readonly IApplicationDbContext _db;

    public GetResidentsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<ResidentSummaryDto>> Handle(
        GetResidentsQuery query, CancellationToken cancellationToken)
    {
        var residents = _db.Residents.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            residents = residents.Where(r => r.LastName.Contains(term) || r.FirstName.Contains(term));
        }

        // On récupère les 5 autonomies, puis on calcule le niveau global en mémoire
        // (le Max sur un enum ne se traduit pas en SQL).
        var rows = await residents
            .OrderBy(r => r.LastName)
            .ThenBy(r => r.FirstName)
            .Select(r => new
            {
                r.Id,
                r.FirstName,
                r.LastName,
                r.RoomNumber,
                r.IsArchived,
                r.FallRisk,
                r.Dependency.Eating,
                r.Dependency.Elimination,
                r.Dependency.Mobility,
                r.Dependency.Dressing,
                r.Dependency.Hygiene,
            })
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => new ResidentSummaryDto(
                x.Id,
                x.FirstName,
                x.LastName,
                x.RoomNumber,
                new[] { x.Eating, x.Elimination, x.Mobility, x.Dressing, x.Hygiene }.Max(),
                x.FallRisk,
                x.IsArchived))
            .ToList();
    }
}
