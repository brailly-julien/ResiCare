using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Common.Security;
using ResiCare.Domain.Entities;

namespace ResiCare.Application.Observations.Commands.AddObservation;

public sealed class AddObservationCommandHandler : ICommandHandler<AddObservationCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public AddObservationCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(AddObservationCommand command, CancellationToken cancellationToken)
    {
        // L'auteur de l'observation est l'utilisateur connecté (issu du jeton JWT), jamais une
        // valeur envoyée par le client : on ne peut pas signer une transmission à la place d'autrui.
        var caregiverId = _currentUser.Id ?? throw new UnauthorizedException("Authentification requise.");

        // On lit le résident SANS suivi : on a juste besoin de son Id et de son état (archivé ?).
        var resident = await _db.Residents.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == command.ResidentId, cancellationToken);
        if (resident is null)
            throw new NotFoundException("Résident", command.ResidentId);

        // La fabrique du Domaine porte la règle métier : interdit sur un résident archivé.
        // -> ResidentArchivedException -> 409 Conflict (via le GlobalExceptionHandler).
        var observation = Observation.Create(resident, caregiverId, command.Category, command.Content);

        _db.Observations.Add(observation);
        await _db.SaveChangesAsync(cancellationToken);

        return observation.Id;
    }
}
