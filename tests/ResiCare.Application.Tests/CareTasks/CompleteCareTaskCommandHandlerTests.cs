using FluentAssertions;
using ResiCare.Application.CareTasks.Commands.CompleteCareTask;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Application.Tests.CareTasks;

public class CompleteCareTaskCommandHandlerTests : DatabaseTestBase
{
    private async Task<(Guid TaskId, Guid CaregiverId)> SeedTaskAsync()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "hash");
        var resident = new Resident("Jeanne", "Lefèvre", new DateOnly(1940, 5, 12),
            new DateOnly(2022, 9, 1), "101", TestData.AnyDependency(), caregiver.Id);
        var task = new CareTask(resident.Id, "Toilette du matin", new DateOnly(2026, 6, 18));

        await using var seed = CreateContext();
        seed.Caregivers.Add(caregiver);
        seed.Residents.Add(resident);
        seed.CareTasks.Add(task);
        await seed.SaveChangesAsync();

        return (task.Id, caregiver.Id);
    }

    [Fact]
    public async Task Handle_MarksTaskAsDone_ByCurrentUser()
    {
        var (taskId, caregiverId) = await SeedTaskAsync();

        await using (var context = CreateContext())
        {
            var handler = new CompleteCareTaskCommandHandler(context, new FakeCurrentUser(caregiverId));
            await handler.Handle(new CompleteCareTaskCommand(taskId), CancellationToken.None);
        }

        await using var assertContext = CreateContext();
        var task = await assertContext.CareTasks.FindAsync(taskId);
        task!.Status.Should().Be(CareTaskStatus.Done);
        task.CompletedByCaregiverId.Should().Be(caregiverId);
    }

    [Fact]
    public async Task Handle_CalledTwice_ThrowsAlreadyCompleted()
    {
        var (taskId, caregiverId) = await SeedTaskAsync();

        await using (var context = CreateContext())
        {
            var handler = new CompleteCareTaskCommandHandler(context, new FakeCurrentUser(caregiverId));
            await handler.Handle(new CompleteCareTaskCommand(taskId), CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var handler = new CompleteCareTaskCommandHandler(context, new FakeCurrentUser(caregiverId));
            var act = async () => await handler.Handle(new CompleteCareTaskCommand(taskId), CancellationToken.None);
            await act.Should().ThrowAsync<CareTaskAlreadyCompletedException>();
        }
    }
}
