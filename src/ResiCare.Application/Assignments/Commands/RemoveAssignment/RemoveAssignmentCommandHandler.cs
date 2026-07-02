using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;

namespace ResiCare.Application.Assignments.Commands.RemoveAssignment;

public sealed class RemoveAssignmentCommandHandler : ICommandHandler<RemoveAssignmentCommand, Unit>
{
    private readonly IApplicationDbContext _db;

    public RemoveAssignmentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Unit> Handle(RemoveAssignmentCommand command, CancellationToken cancellationToken)
    {
        var assignment = await _db.Assignments
            .FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);
        if (assignment is null)
            throw new NotFoundException("Affectation", command.Id);

        _db.Assignments.Remove(assignment);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
