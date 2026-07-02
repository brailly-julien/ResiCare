using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Common.Security;
using ResiCare.Domain.Entities;

namespace ResiCare.Application.Prescriptions.Commands.RecordAdministration;

public sealed class RecordAdministrationCommandHandler : ICommandHandler<RecordAdministrationCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public RecordAdministrationCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(RecordAdministrationCommand command, CancellationToken cancellationToken)
    {
        var caregiverId = _currentUser.Id ?? throw new UnauthorizedException("Authentification requise.");

        var prescription = await _db.Prescriptions.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == command.PrescriptionId, cancellationToken);
        if (prescription is null)
            throw new NotFoundException("Prescription", command.PrescriptionId);

        // La fabrique du Domaine porte la règle : pas d'administration sur prescription arrêtée.
        // -> PrescriptionDiscontinuedException -> 409 Conflict.
        var administration = MedicationAdministration.Record(
            prescription, caregiverId, command.Status, command.Notes);

        _db.MedicationAdministrations.Add(administration);
        await _db.SaveChangesAsync(cancellationToken);

        return administration.Id;
    }
}
