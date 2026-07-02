using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Domain.ValueObjects;

namespace ResiCare.Application.Residents.Commands.UpdateResident;

public sealed class UpdateResidentCommandHandler : ICommandHandler<UpdateResidentCommand, Unit>
{
    private readonly IApplicationDbContext _db;

    public UpdateResidentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Unit> Handle(UpdateResidentCommand command, CancellationToken cancellationToken)
    {
        var resident = await _db.Residents
            .FirstOrDefaultAsync(r => r.Id == command.Id, cancellationToken);
        if (resident is null)
            throw new NotFoundException("Résident", command.Id);

        var referentExists = await _db.Caregivers
            .AnyAsync(c => c.Id == command.ReferentCaregiverId, cancellationToken);
        if (!referentExists)
            throw new NotFoundException("Soignant référent", command.ReferentCaregiverId);

        var dependency = new DependencyProfile(
            command.Eating, command.Elimination, command.Mobility, command.Dressing, command.Hygiene);

        resident.UpdateDetails(
            command.FirstName, command.LastName, command.RoomNumber, dependency, command.ReferentCaregiverId);
        resident.SetRisks(command.FallRisk, command.PressureSoreRisk, command.MalnutritionRisk);
        resident.SetPersonalInfo(
            command.AttendingPhysician, command.EmergencyContactName, command.EmergencyContactPhone,
            command.Occupation, command.Interests, command.Family);

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
