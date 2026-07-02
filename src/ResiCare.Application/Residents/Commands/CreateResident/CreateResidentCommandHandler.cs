using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Domain.Entities;
using ResiCare.Domain.ValueObjects;

namespace ResiCare.Application.Residents.Commands.CreateResident;

public sealed class CreateResidentCommandHandler : ICommandHandler<CreateResidentCommand, Guid>
{
    private readonly IApplicationDbContext _db;

    public CreateResidentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateResidentCommand command, CancellationToken cancellationToken)
    {
        var referentExists = await _db.Caregivers
            .AnyAsync(caregiver => caregiver.Id == command.ReferentCaregiverId, cancellationToken);
        if (!referentExists)
            throw new NotFoundException("Soignant référent", command.ReferentCaregiverId);

        var dependency = new DependencyProfile(
            command.Eating, command.Elimination, command.Mobility, command.Dressing, command.Hygiene);

        var resident = new Resident(
            command.FirstName, command.LastName, command.BirthDate, command.AdmissionDate,
            command.RoomNumber, dependency, command.ReferentCaregiverId);

        resident.SetRisks(command.FallRisk, command.PressureSoreRisk, command.MalnutritionRisk);
        resident.SetPersonalInfo(
            command.AttendingPhysician, command.EmergencyContactName, command.EmergencyContactPhone,
            command.Occupation, command.Interests, command.Family);

        _db.Residents.Add(resident);
        await _db.SaveChangesAsync(cancellationToken);

        return resident.Id;
    }
}
