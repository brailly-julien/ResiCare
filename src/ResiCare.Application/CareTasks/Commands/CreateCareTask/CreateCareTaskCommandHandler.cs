using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Domain.Entities;

namespace ResiCare.Application.CareTasks.Commands.CreateCareTask;

public sealed class CreateCareTaskCommandHandler : ICommandHandler<CreateCareTaskCommand, Guid>
{
    private readonly IApplicationDbContext _db;

    public CreateCareTaskCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateCareTaskCommand command, CancellationToken cancellationToken)
    {
        var residentExists = await _db.Residents
            .AnyAsync(r => r.Id == command.ResidentId, cancellationToken);
        if (!residentExists)
            throw new NotFoundException("Résident", command.ResidentId);

        var task = new CareTask(command.ResidentId, command.Label, command.ScheduledDate);

        _db.CareTasks.Add(task);
        await _db.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}
