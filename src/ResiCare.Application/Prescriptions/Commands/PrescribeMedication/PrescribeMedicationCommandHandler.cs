using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Domain.Entities;

namespace ResiCare.Application.Prescriptions.Commands.PrescribeMedication;

public sealed class PrescribeMedicationCommandHandler : ICommandHandler<PrescribeMedicationCommand, Guid>
{
    private readonly IApplicationDbContext _db;

    public PrescribeMedicationCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(PrescribeMedicationCommand command, CancellationToken cancellationToken)
    {
        var residentExists = await _db.Residents.AnyAsync(r => r.Id == command.ResidentId, cancellationToken);
        if (!residentExists)
            throw new NotFoundException("Résident", command.ResidentId);

        var prescription = new Prescription(
            command.ResidentId, command.MedicationName, command.Dosage, command.Posology,
            command.Route, command.StartDate, command.EndDate, command.Instructions);

        _db.Prescriptions.Add(prescription);
        await _db.SaveChangesAsync(cancellationToken);

        return prescription.Id;
    }
}
