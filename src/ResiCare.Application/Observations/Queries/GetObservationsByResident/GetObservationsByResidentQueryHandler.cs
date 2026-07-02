using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.Observations.Queries.GetObservationsByResident;

public sealed class GetObservationsByResidentQueryHandler
    : IQueryHandler<GetObservationsByResidentQuery, PagedResult<ObservationDto>>
{
    private readonly IApplicationDbContext _db;

    public GetObservationsByResidentQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<ObservationDto>> Handle(
        GetObservationsByResidentQuery query, CancellationToken cancellationToken)
    {
        // Garde-fous sur les paramètres de pagination.
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 10 : query.PageSize;

        var observations = _db.Observations.AsNoTracking()
            .Where(o => o.ResidentId == query.ResidentId);

        if (query.Category is not null)
            observations = observations.Where(o => o.Category == query.Category);

        var totalCount = await observations.CountAsync(cancellationToken);

        var items = await observations
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new ObservationDto(o.Id, o.Category, o.Content, o.CreatedAt, o.CaregiverId))
            .ToListAsync(cancellationToken);

        return new PagedResult<ObservationDto>(items, totalCount, page, pageSize);
    }
}
