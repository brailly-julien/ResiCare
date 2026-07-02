using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.TimeClock.Commands.ClockIn;
using ResiCare.Application.TimeClock.Commands.ClockOut;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.TimeClock;

public class ClockOutCommandHandlerTests : DatabaseTestBase
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
    public async Task Handle_ClosesOpenEntry()
    {
        var caregiverId = await SeedCaregiverAsync();
        await using (var context = CreateContext())
        {
            var clockIn = new ClockInCommandHandler(context, new FakeCurrentUser(caregiverId));
            await clockIn.Handle(new ClockInCommand(), CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var clockOut = new ClockOutCommandHandler(context, new FakeCurrentUser(caregiverId));
            await clockOut.Handle(new ClockOutCommand(), CancellationToken.None);
        }

        await using var assert = CreateContext();
        var entry = await assert.TimeEntries.FirstAsync(e => e.CaregiverId == caregiverId);
        entry.IsOpen.Should().BeFalse();
        entry.ClockOutAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenNoOpenShift_ThrowsConflict()
    {
        var caregiverId = await SeedCaregiverAsync();

        await using var context = CreateContext();
        var handler = new ClockOutCommandHandler(context, new FakeCurrentUser(caregiverId));
        var act = async () => await handler.Handle(new ClockOutCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }
}
