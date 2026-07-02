using FluentAssertions;
using ResiCare.Application.Assignments.Commands.AssignCaregiver;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.Assignments;

public class AssignCaregiverCommandHandlerTests : DatabaseTestBase
{
    private static readonly DateOnly Today = new(2026, 6, 22);

    private async Task<(Guid CaregiverId, Guid ResidentId)> SeedAsync()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "hash");
        var resident = new Resident("Jeanne", "Lefèvre", new DateOnly(1940, 5, 12),
            new DateOnly(2022, 9, 1), "101", TestData.AnyDependency(), caregiver.Id);
        await using var seed = CreateContext();
        seed.Caregivers.Add(caregiver);
        seed.Residents.Add(resident);
        await seed.SaveChangesAsync();
        return (caregiver.Id, resident.Id);
    }

    [Fact]
    public async Task Handle_CreatesAssignment()
    {
        var (caregiverId, residentId) = await SeedAsync();

        Guid id;
        await using (var context = CreateContext())
        {
            var handler = new AssignCaregiverCommandHandler(context);
            id = await handler.Handle(
                new AssignCaregiverCommand(caregiverId, residentId, Today, Shift.Morning), CancellationToken.None);
        }

        await using var assert = CreateContext();
        var assignment = await assert.Assignments.FindAsync(id);
        assignment.Should().NotBeNull();
        assignment!.CaregiverId.Should().Be(caregiverId);
        assignment.ResidentId.Should().Be(residentId);
    }

    [Fact]
    public async Task Handle_WhenResidentNotFound_ThrowsNotFound()
    {
        var (caregiverId, _) = await SeedAsync();

        await using var context = CreateContext();
        var handler = new AssignCaregiverCommandHandler(context);
        var act = async () => await handler.Handle(
            new AssignCaregiverCommand(caregiverId, Guid.NewGuid(), Today, Shift.Morning), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenCaregiverNotFound_ThrowsNotFound()
    {
        var (_, residentId) = await SeedAsync();

        await using var context = CreateContext();
        var handler = new AssignCaregiverCommandHandler(context);
        var act = async () => await handler.Handle(
            new AssignCaregiverCommand(Guid.NewGuid(), residentId, Today, Shift.Morning), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenAlreadyAssigned_ThrowsConflict()
    {
        var (caregiverId, residentId) = await SeedAsync();
        await using (var context = CreateContext())
        {
            var handler = new AssignCaregiverCommandHandler(context);
            await handler.Handle(
                new AssignCaregiverCommand(caregiverId, residentId, Today, Shift.Morning), CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var handler = new AssignCaregiverCommandHandler(context);
            var act = async () => await handler.Handle(
                new AssignCaregiverCommand(caregiverId, residentId, Today, Shift.Morning), CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>();
        }
    }
}
