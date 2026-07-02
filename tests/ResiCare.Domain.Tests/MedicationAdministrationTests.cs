using FluentAssertions;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Tests;

public class MedicationAdministrationTests
{
    private static Prescription ActivePrescription() =>
        new(Guid.NewGuid(), "Paracétamol", "1000 mg", "3x/jour", MedicationRoute.Oral,
            new DateOnly(2026, 6, 22), null, null);

    [Fact]
    public void Record_OnActivePrescription_CreatesAdministration()
    {
        var prescription = ActivePrescription();
        var caregiverId = Guid.NewGuid();

        var admin = MedicationAdministration.Record(prescription, caregiverId, AdministrationStatus.Given, "RAS");

        admin.PrescriptionId.Should().Be(prescription.Id);
        admin.CaregiverId.Should().Be(caregiverId);
        admin.Status.Should().Be(AdministrationStatus.Given);
    }

    [Fact]
    public void Record_OnDiscontinuedPrescription_Throws()
    {
        var prescription = ActivePrescription();
        prescription.Discontinue();

        Action act = () => MedicationAdministration.Record(
            prescription, Guid.NewGuid(), AdministrationStatus.Given, null);

        act.Should().Throw<PrescriptionDiscontinuedException>();
    }
}
