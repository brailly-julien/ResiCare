using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.Prescriptions.Commands.DiscontinuePrescription;

public sealed class DiscontinuePrescriptionCommandHandler : ICommandHandler<DiscontinuePrescriptionCommand, Unit>
{
    private readonly IApplicationDbContext _db;

    public DiscontinuePrescriptionCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Unit> Handle(DiscontinuePrescriptionCommand command, CancellationToken cancellationToken)
    {
        // Écriture : on charge la prescription SUIVIE pour que Discontinue() génère l'UPDATE.
        var prescription = await _db.Prescriptions
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);
        if (prescription is null)
            throw new NotFoundException("Prescription", command.Id);

        prescription.Discontinue(); // règle « déjà arrêtée » portée par le Domaine -> 409
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
