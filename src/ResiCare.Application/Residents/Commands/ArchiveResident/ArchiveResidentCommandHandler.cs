using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.Residents.Commands.ArchiveResident;

public sealed class ArchiveResidentCommandHandler : ICommandHandler<ArchiveResidentCommand, Unit>
{
    private readonly IApplicationDbContext _db;

    public ArchiveResidentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Unit> Handle(ArchiveResidentCommand command, CancellationToken cancellationToken)
    {
        var resident = await _db.Residents
            .FirstOrDefaultAsync(r => r.Id == command.Id, cancellationToken);

        if (resident is null)
            throw new NotFoundException("Résident", command.Id);

        resident.Archive(); // soft delete : on conserve tout l'historique
        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
