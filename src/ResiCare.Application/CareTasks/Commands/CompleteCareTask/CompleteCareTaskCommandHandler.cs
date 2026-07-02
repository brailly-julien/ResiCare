using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Common.Security;

namespace ResiCare.Application.CareTasks.Commands.CompleteCareTask;

public sealed class CompleteCareTaskCommandHandler : ICommandHandler<CompleteCareTaskCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public CompleteCareTaskCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(CompleteCareTaskCommand command, CancellationToken cancellationToken)
    {
        // Le soignant qui valide la tâche est l'utilisateur connecté (jeton JWT).
        var caregiverId = _currentUser.Id ?? throw new UnauthorizedException("Authentification requise.");

        // Écriture : on charge la tâche SUIVIE pour que Complete() génère l'UPDATE.
        var task = await _db.CareTasks
            .FirstOrDefaultAsync(t => t.Id == command.TaskId, cancellationToken);
        if (task is null)
            throw new NotFoundException("Tâche de soin", command.TaskId);

        // Règle métier portée par le Domaine : une tâche ne se complète qu'une fois.
        // 2e appel -> CareTaskAlreadyCompletedException -> 409 Conflict.
        task.Complete(caregiverId);

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
