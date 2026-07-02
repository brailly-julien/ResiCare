using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.TimeClock;

/// <summary>Vérifie l'index unique filtré « un seul pointage ouvert par soignant ».</summary>
public class TimeEntryConstraintsTests : DatabaseTestBase
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
    public async Task TwoOpenEntries_ForSameCaregiver_ViolateUniqueIndex()
    {
        var caregiverId = await SeedCaregiverAsync();

        await using var ctx = CreateContext();
        ctx.TimeEntries.Add(TimeEntry.ClockIn(caregiverId));
        ctx.TimeEntries.Add(TimeEntry.ClockIn(caregiverId)); // 2e pointage OUVERT -> interdit

        var act = async () => await ctx.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task ClosedEntry_DoesNotBlockNewOpenEntry()
    {
        var caregiverId = await SeedCaregiverAsync();

        await using var ctx = CreateContext();
        var first = TimeEntry.ClockIn(caregiverId);
        first.ClockOut(); // clôturé -> ne compte plus dans le filtre de l'index
        ctx.TimeEntries.Add(first);
        await ctx.SaveChangesAsync();

        ctx.TimeEntries.Add(TimeEntry.ClockIn(caregiverId)); // nouveau pointage ouvert -> autorisé
        var act = async () => await ctx.SaveChangesAsync();

        await act.Should().NotThrowAsync();
    }
}
