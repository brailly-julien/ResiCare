using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Observations.Commands.AddObservation;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Application.Tests.Observations;

public class AddObservationCommandHandlerTests : DatabaseTestBase
{
    [Fact]
    public async Task Handle_OnArchivedResident_ThrowsResidentArchived()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "hash");
        var resident = new Resident("Jeanne", "Lefèvre", new DateOnly(1940, 5, 12),
            new DateOnly(2022, 9, 1), "101", TestData.AnyDependency(), caregiver.Id);
        resident.Archive();
        await using (var seed = CreateContext())
        {
            seed.Caregivers.Add(caregiver);
            seed.Residents.Add(resident);
            await seed.SaveChangesAsync();
        }

        await using var context = CreateContext();
        var handler = new AddObservationCommandHandler(context, new FakeCurrentUser(caregiver.Id));
        var command = new AddObservationCommand(resident.Id, ObservationCategory.Care, "RAS");
        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResidentArchivedException>();
    }

    [Fact]
    public async Task Handle_OnActiveResident_PersistsObservationAuthoredByCurrentUser()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "hash");
        var resident = new Resident("Jeanne", "Lefèvre", new DateOnly(1940, 5, 12),
            new DateOnly(2022, 9, 1), "101", TestData.AnyDependency(), caregiver.Id);
        await using (var seed = CreateContext())
        {
            seed.Caregivers.Add(caregiver);
            seed.Residents.Add(resident);
            await seed.SaveChangesAsync();
        }

        await using (var context = CreateContext())
        {
            var handler = new AddObservationCommandHandler(context, new FakeCurrentUser(caregiver.Id));
            await handler.Handle(
                new AddObservationCommand(resident.Id, ObservationCategory.Nutrition, "A bien mangé."),
                CancellationToken.None);
        }

        await using var assertContext = CreateContext();
        var observations = await assertContext.Observations
            .Where(o => o.ResidentId == resident.Id)
            .ToListAsync();
        observations.Should().ContainSingle();
        // L'auteur enregistré est bien l'utilisateur connecté (et non une valeur du client).
        observations[0].CaregiverId.Should().Be(caregiver.Id);
    }
}
