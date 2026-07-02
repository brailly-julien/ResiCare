using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.Prescriptions.Queries.GetPrescriptionsByResident;

public sealed class GetPrescriptionsByResidentQueryHandler
    : IQueryHandler<GetPrescriptionsByResidentQuery, IReadOnlyList<PrescriptionDto>>
{
    private readonly IApplicationDbContext _db;

    public GetPrescriptionsByResidentQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<PrescriptionDto>> Handle(
        GetPrescriptionsByResidentQuery query, CancellationToken cancellationToken)
    {
        return await _db.Prescriptions.AsNoTracking()
            .Where(p => p.ResidentId == query.ResidentId)
            .OrderBy(p => p.IsDiscontinued)
            .ThenBy(p => p.MedicationName)
            .Select(p => new PrescriptionDto(
                p.Id, p.ResidentId, p.MedicationName, p.Dosage, p.Posology, p.Route,
                p.StartDate, p.EndDate, p.Instructions, p.IsDiscontinued))
            .ToListAsync(cancellationToken);
    }
}
