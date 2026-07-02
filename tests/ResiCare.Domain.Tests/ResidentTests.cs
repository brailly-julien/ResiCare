using FluentAssertions;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Tests;

public class ResidentTests
{
    private static Resident CreateValidResident() =>
        new("Jeanne", "Lefèvre",
            birthDate: new DateOnly(1940, 5, 12),
            admissionDate: new DateOnly(2022, 9, 1),
            roomNumber: "101",
            dependency: TestData.AnyDependency(),
            referentCaregiverId: Guid.NewGuid());

    [Fact]
    public void Constructor_WithValidData_CreatesActiveResident()
    {
        var resident = CreateValidResident();

        resident.Id.Should().NotBe(Guid.Empty);
        resident.IsArchived.Should().BeFalse();
        resident.FirstName.Should().Be("Jeanne");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithBlankFirstName_ThrowsDomainException(string? firstName)
    {
        Action act = () => new Resident(
            firstName!, "Lefèvre", new DateOnly(1940, 5, 12), new DateOnly(2022, 9, 1),
            "101", TestData.AnyDependency(), Guid.NewGuid());

        act.Should().Throw<DomainException>().WithMessage("*prénom*");
    }

    [Fact]
    public void Constructor_WithAdmissionBeforeBirth_ThrowsDomainException()
    {
        Action act = () => new Resident(
            "Jeanne", "Lefèvre",
            birthDate: new DateOnly(2022, 1, 1),
            admissionDate: new DateOnly(2020, 1, 1),
            "101", TestData.AnyDependency(), Guid.NewGuid());

        act.Should().Throw<DomainException>().WithMessage("*admission*");
    }

    [Fact]
    public void Archive_SetsIsArchivedToTrue()
    {
        var resident = CreateValidResident();

        resident.Archive();

        resident.IsArchived.Should().BeTrue();
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesFields()
    {
        var resident = CreateValidResident();
        var newReferent = Guid.NewGuid();

        resident.UpdateDetails("Jean", "Dupont", "202", TestData.AnyDependency(), newReferent);

        resident.FirstName.Should().Be("Jean");
        resident.RoomNumber.Should().Be("202");
        resident.ReferentCaregiverId.Should().Be(newReferent);
    }

    [Fact]
    public void SetRisks_And_SetPersonalInfo_UpdateOptionalFields()
    {
        var resident = CreateValidResident();

        resident.SetRisks(RiskLevel.High, RiskLevel.None, RiskLevel.High);
        resident.SetPersonalInfo("Dr X", "Contact", "06 00 00 00 00", "Institutrice", "Lecture", "2 enfants");

        resident.FallRisk.Should().Be(RiskLevel.High);
        resident.MalnutritionRisk.Should().Be(RiskLevel.High);
        resident.AttendingPhysician.Should().Be("Dr X");
        resident.Occupation.Should().Be("Institutrice");
    }

    [Fact]
    public void SetPersonalInfo_WithBlankValues_StoresNull()
    {
        var resident = CreateValidResident();

        resident.SetPersonalInfo("   ", null, "", null, null, null);

        resident.AttendingPhysician.Should().BeNull();
        resident.EmergencyContactPhone.Should().BeNull();
    }
}
