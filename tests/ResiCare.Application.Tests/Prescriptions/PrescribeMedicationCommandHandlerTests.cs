using FluentAssertions;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Prescriptions.Commands.PrescribeMedication;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.Prescriptions;

public class PrescribeMedicationCommandHandlerTests : DatabaseTestBase
{
    private static readonly DateOnly Today = new(2026, 6, 22);

    private async Task<Guid> SeedResidentAsync()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "hash");
        var resident = new Resident("Jeanne", "Lefèvre", new DateOnly(1940, 5, 12),
            new DateOnly(2022, 9, 1), "101", TestData.AnyDependency(), caregiver.Id);
        await using var seed = CreateContext();
        seed.Caregivers.Add(caregiver);
        seed.Residents.Add(resident);
        await seed.SaveChangesAsync();
        return resident.Id;
    }

    private static PrescribeMedicationCommand Command(Guid residentId) =>
        new(residentId, "Paracétamol", "1000 mg", "3x/jour", MedicationRoute.Oral, Today, null, null);

    [Fact]
    public async Task Handle_PersistsPrescription()
    {
        var residentId = await SeedResidentAsync();

        Guid id;
        await using (var context = CreateContext())
        {
            var handler = new PrescribeMedicationCommandHandler(context);
            id = await handler.Handle(Command(residentId), CancellationToken.None);
        }

        await using var assert = CreateContext();
        var prescription = await assert.Prescriptions.FindAsync(id);
        prescription.Should().NotBeNull();
        prescription!.MedicationName.Should().Be("Paracétamol");
        prescription.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenResidentNotFound_ThrowsNotFound()
    {
        await using var context = CreateContext();
        var handler = new PrescribeMedicationCommandHandler(context);

        var act = async () => await handler.Handle(Command(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
