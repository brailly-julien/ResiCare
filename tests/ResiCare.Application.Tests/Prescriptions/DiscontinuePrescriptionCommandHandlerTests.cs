using FluentAssertions;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Prescriptions.Commands.DiscontinuePrescription;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.Prescriptions;

public class DiscontinuePrescriptionCommandHandlerTests : DatabaseTestBase
{
    [Fact]
    public async Task Handle_MarksPrescriptionDiscontinued()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "hash");
        var resident = new Resident("Jeanne", "Lefèvre", new DateOnly(1940, 5, 12),
            new DateOnly(2022, 9, 1), "101", TestData.AnyDependency(), caregiver.Id);
        var prescription = new Prescription(resident.Id, "Paracétamol", "1000 mg", "3x/jour",
            MedicationRoute.Oral, new DateOnly(2026, 6, 22), null, null);
        await using (var seed = CreateContext())
        {
            seed.Caregivers.Add(caregiver);
            seed.Residents.Add(resident);
            seed.Prescriptions.Add(prescription);
            await seed.SaveChangesAsync();
        }

        await using (var context = CreateContext())
        {
            var handler = new DiscontinuePrescriptionCommandHandler(context);
            await handler.Handle(new DiscontinuePrescriptionCommand(prescription.Id), CancellationToken.None);
        }

        await using var assert = CreateContext();
        var updated = await assert.Prescriptions.FindAsync(prescription.Id);
        updated!.IsDiscontinued.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenNotFound_ThrowsNotFound()
    {
        await using var context = CreateContext();
        var handler = new DiscontinuePrescriptionCommandHandler(context);

        var act = async () => await handler.Handle(new DiscontinuePrescriptionCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
