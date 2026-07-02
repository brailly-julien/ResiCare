using FluentAssertions;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Residents.Commands.CreateResident;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.Residents;

public class CreateResidentCommandHandlerTests : DatabaseTestBase
{
    private static CreateResidentCommand CommandWithReferent(Guid referentId) =>
        new(
            "Jeanne", "Lefèvre", new DateOnly(1940, 5, 12), new DateOnly(2022, 9, 1), "101",
            AutonomyLevel.Independent, AutonomyLevel.Independent, AutonomyLevel.Independent,
            AutonomyLevel.Independent, AutonomyLevel.Independent,
            FallRisk: RiskLevel.None, PressureSoreRisk: RiskLevel.None, MalnutritionRisk: RiskLevel.None,
            AttendingPhysician: null, EmergencyContactName: null, EmergencyContactPhone: null,
            Occupation: null, Interests: null, Family: null,
            referentId);

    [Fact]
    public async Task Handle_WhenReferentDoesNotExist_ThrowsNotFound()
    {
        await using var context = CreateContext();
        var handler = new CreateResidentCommandHandler(context);

        var act = async () => await handler.Handle(CommandWithReferent(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithValidReferent_PersistsResident()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "hash");
        await using (var seed = CreateContext())
        {
            seed.Caregivers.Add(caregiver);
            await seed.SaveChangesAsync();
        }

        Guid newId;
        await using (var context = CreateContext())
        {
            var handler = new CreateResidentCommandHandler(context);
            newId = await handler.Handle(CommandWithReferent(caregiver.Id), CancellationToken.None);
        }

        await using var assertContext = CreateContext();
        var resident = await assertContext.Residents.FindAsync(newId);
        resident.Should().NotBeNull();
        resident!.LastName.Should().Be("Lefèvre");
    }
}
