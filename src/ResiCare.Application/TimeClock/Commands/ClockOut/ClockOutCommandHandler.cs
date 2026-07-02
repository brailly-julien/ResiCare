using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Common.Security;

namespace ResiCare.Application.TimeClock.Commands.ClockOut;

public sealed class ClockOutCommandHandler : ICommandHandler<ClockOutCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public ClockOutCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(ClockOutCommand command, CancellationToken cancellationToken)
    {
        var caregiverId = _currentUser.Id ?? throw new UnauthorizedException("Authentification requise.");

        // On charge le pointage ouvert (suivi) pour que ClockOut() génère l'UPDATE.
        var entry = await _db.TimeEntries
            .Where(e => e.CaregiverId == caregiverId && e.ClockOutAt == null)
            .OrderByDescending(e => e.ClockInAt)
            .FirstOrDefaultAsync(cancellationToken);
        if (entry is null)
            throw new ConflictException("Aucun pointage en cours à clôturer.");

        entry.ClockOut(); // règle « déjà clôturé » portée par le Domaine (ici toujours ouvert)
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
