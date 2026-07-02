using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Domain.Entities;

namespace ResiCare.Application.Assignments.Commands.AssignCaregiver;

public sealed class AssignCaregiverCommandHandler : ICommandHandler<AssignCaregiverCommand, Guid>
{
    private readonly IApplicationDbContext _db;

    public AssignCaregiverCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(AssignCaregiverCommand command, CancellationToken cancellationToken)
    {
        var caregiverExists = await _db.Caregivers.AnyAsync(c => c.Id == command.CaregiverId, cancellationToken);
        if (!caregiverExists)
            throw new NotFoundException("Soignant", command.CaregiverId);

        var residentExists = await _db.Residents.AnyAsync(r => r.Id == command.ResidentId, cancellationToken);
        if (!residentExists)
            throw new NotFoundException("Résident", command.ResidentId);

        // Règle : un seul soignant par résident et par poste sur une date donnée.
        var alreadyAssigned = await _db.Assignments.AnyAsync(
            a => a.ResidentId == command.ResidentId && a.Date == command.Date && a.Shift == command.Shift,
            cancellationToken);
        if (alreadyAssigned)
            throw new ConflictException("Ce résident est déjà affecté pour ce poste.");

        var assignment = new Assignment(command.CaregiverId, command.ResidentId, command.Date, command.Shift);
        _db.Assignments.Add(assignment);
        await _db.SaveChangesAsync(cancellationToken);

        return assignment.Id;
    }
}
