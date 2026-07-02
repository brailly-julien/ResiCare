using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Common.Security;
using ResiCare.Domain.Entities;

namespace ResiCare.Application.TimeClock.Commands.ClockIn;

public sealed class ClockInCommandHandler : ICommandHandler<ClockInCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public ClockInCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(ClockInCommand command, CancellationToken cancellationToken)
    {
        var caregiverId = _currentUser.Id ?? throw new UnauthorizedException("Authentification requise.");

        // Règle inter-entités : pas deux pointages ouverts en même temps pour un même soignant.
        var hasOpenShift = await _db.TimeEntries
            .AnyAsync(e => e.CaregiverId == caregiverId && e.ClockOutAt == null, cancellationToken);
        if (hasOpenShift)
            throw new ConflictException("Un pointage est déjà en cours : pointez d'abord votre départ.");

        var entry = TimeEntry.ClockIn(caregiverId);
        _db.TimeEntries.Add(entry);
        await _db.SaveChangesAsync(cancellationToken);

        return entry.Id;
    }
}
