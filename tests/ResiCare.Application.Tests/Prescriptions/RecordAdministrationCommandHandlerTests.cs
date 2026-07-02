using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Prescriptions.Commands.RecordAdministration;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Application.Tests.Prescriptions;

public class RecordAdministrationCommandHandlerTests : DatabaseTestBase
{
    private static readonly DateOnly Today = new(2026, 6, 22);

    private async Task<(Guid CaregiverId, Guid PrescriptionId)> SeedAsync(bool discontinued = false)
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "hash");
        var resident = new Resident("Jeanne", "Lefèvre", new DateOnly(1940, 5, 12),
            new DateOnly(2022, 9, 1), "101", TestData.AnyDependency(), caregiver.Id);
        var prescription = new Prescription(resident.Id, "Paracétamol", "1000 mg", "3x/jour",
            MedicationRoute.Oral, Today, null, null);
        if (discontinued)
            prescription.Discontinue();

        await using var seed = CreateContext();
        seed.Caregivers.Add(caregiver);
        seed.Residents.Add(resident);
        seed.Prescriptions.Add(prescription);
        await seed.SaveChangesAsync();
        return (caregiver.Id, prescription.Id);
    }

    [Fact]
    public async Task Handle_RecordsAdministrationByCurrentUser()
    {
        var (caregiverId, prescriptionId) = await SeedAsync();

        await using (var context = CreateContext())
        {
            var handler = new RecordAdministrationCommandHandler(context, new FakeCurrentUser(caregiverId));
            await handler.Handle(
                new RecordAdministrationCommand(prescriptionId, AdministrationStatus.Given, null), CancellationToken.None);
        }

        await using var assert = CreateContext();
        var admin = await assert.MedicationAdministrations.FirstAsync(a => a.PrescriptionId == prescriptionId);
        admin.CaregiverId.Should().Be(caregiverId);
        admin.Status.Should().Be(AdministrationStatus.Given);
    }

    [Fact]
    public async Task Handle_OnDiscontinuedPrescription_ThrowsDiscontinued()
    {
        var (caregiverId, prescriptionId) = await SeedAsync(discontinued: true);

        await using var context = CreateContext();
        var handler = new RecordAdministrationCommandHandler(context, new FakeCurrentUser(caregiverId));
        var act = async () => await handler.Handle(
            new RecordAdministrationCommand(prescriptionId, AdministrationStatus.Given, null), CancellationToken.None);

        await act.Should().ThrowAsync<PrescriptionDiscontinuedException>();
    }
}
