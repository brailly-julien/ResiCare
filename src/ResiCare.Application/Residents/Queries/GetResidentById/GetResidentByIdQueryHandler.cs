using Microsoft.EntityFrameworkCore;
using ResiCare.Application.CareTasks;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Observations;

namespace ResiCare.Application.Residents.Queries.GetResidentById;

public sealed class GetResidentByIdQueryHandler : IQueryHandler<GetResidentByIdQuery, ResidentDashboardDto>
{
    private readonly IApplicationDbContext _db;

    public GetResidentByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<ResidentDashboardDto> Handle(GetResidentByIdQuery query, CancellationToken cancellationToken)
    {
        var resident = await _db.Residents.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == query.Id, cancellationToken);

        if (resident is null)
            throw new NotFoundException("Résident", query.Id);

        var recentObservations = await _db.Observations.AsNoTracking()
            .Where(o => o.ResidentId == query.Id)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new ObservationDto(o.Id, o.Category, o.Content, o.CreatedAt, o.CaregiverId))
            .ToListAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var todayTasks = await _db.CareTasks.AsNoTracking()
            .Where(t => t.ResidentId == query.Id && t.ScheduledDate == today)
            .OrderBy(t => t.Label)
            .Select(t => new CareTaskDto(
                t.Id, t.Label, t.ScheduledDate, t.Status, t.CompletedByCaregiverId, t.CompletedAt))
            .ToListAsync(cancellationToken);

        var dependency = new DependencyDto(
            resident.Dependency.Eating,
            resident.Dependency.Elimination,
            resident.Dependency.Mobility,
            resident.Dependency.Dressing,
            resident.Dependency.Hygiene);

        return new ResidentDashboardDto(
            resident.Id,
            resident.FirstName,
            resident.LastName,
            resident.BirthDate,
            resident.AdmissionDate,
            resident.RoomNumber,
            dependency,
            resident.ReferentCaregiverId,
            resident.IsArchived,
            resident.FallRisk,
            resident.PressureSoreRisk,
            resident.MalnutritionRisk,
            resident.AttendingPhysician,
            resident.EmergencyContactName,
            resident.EmergencyContactPhone,
            resident.Occupation,
            resident.Interests,
            resident.Family,
            recentObservations,
            todayTasks);
    }
}
