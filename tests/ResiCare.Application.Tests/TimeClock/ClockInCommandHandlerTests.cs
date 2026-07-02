using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.TimeClock.Commands.ClockIn;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.TimeClock;

public class ClockInCommandHandlerTests : DatabaseTestBase
{
    private async Task<Guid> SeedCaregiverAsync()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "hash");
        await using var seed = CreateContext();
        seed.Caregivers.Add(caregiver);
        await seed.SaveChangesAsync();
        return caregiver.Id;
    }

    [Fact]
    public async Task Handle_CreatesOpenEntry()
    {
        var caregiverId = await SeedCaregiverAsync();

        Guid entryId;
        await using (var context = CreateContext())
        {
            var handler = new ClockInCommandHandler(context, new FakeCurrentUser(caregiverId));
            entryId = await handler.Handle(new ClockInCommand(), CancellationToken.None);
        }

        await using var assert = CreateContext();
        var entry = await assert.TimeEntries.FindAsync(entryId);
        entry.Should().NotBeNull();
        entry!.CaregiverId.Should().Be(caregiverId);
        entry.IsOpen.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenShiftAlreadyOpen_ThrowsConflict()
    {
        var caregiverId = await SeedCaregiverAsync();
        await using (var context = CreateContext())
        {
            var handler = new ClockInCommandHandler(context, new FakeCurrentUser(caregiverId));
            await handler.Handle(new ClockInCommand(), CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var handler = new ClockInCommandHandler(context, new FakeCurrentUser(caregiverId));
            var act = async () => await handler.Handle(new ClockInCommand(), CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>();
        }
    }
}
