using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.Prescriptions.Queries.GetAdministrationsByResidentAndDate;

public sealed class GetAdministrationsByResidentAndDateQueryHandler
    : IQueryHandler<GetAdministrationsByResidentAndDateQuery, IReadOnlyList<AdministrationDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAdministrationsByResidentAndDateQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<AdministrationDto>> Handle(
        GetAdministrationsByResidentAndDateQuery query, CancellationToken cancellationToken)
    {
        var start = query.Date.ToDateTime(TimeOnly.MinValue);
        var end = start.AddDays(1);

        // Jointure administration <-> prescription (FK par Id) : on filtre par résident + date.
        return await (
            from m in _db.MedicationAdministrations.AsNoTracking()
            join p in _db.Prescriptions.AsNoTracking() on m.PrescriptionId equals p.Id
            where p.ResidentId == query.ResidentId && m.AdministeredAt >= start && m.AdministeredAt < end
            orderby m.AdministeredAt descending
            select new AdministrationDto(
                m.Id, m.PrescriptionId, p.MedicationName, m.CaregiverId, m.AdministeredAt, m.Status, m.Notes))
            .ToListAsync(cancellationToken);
    }
}
