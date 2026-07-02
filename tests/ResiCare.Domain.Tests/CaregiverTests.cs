using FluentAssertions;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Tests;

public class CaregiverTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesCaregiver()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Manager,
            "paul.durand@resicare.local", "hash");

        caregiver.Id.Should().NotBe(Guid.Empty);
        caregiver.Role.Should().Be(CaregiverRole.Manager);
        caregiver.FullName.Should().Be("Paul Durand");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithBlankLastName_ThrowsDomainException(string? lastName)
    {
        Action act = () => new Caregiver("Paul", lastName!, CaregiverRole.Caregiver,
            "paul.durand@resicare.local", "hash");

        act.Should().Throw<DomainException>().WithMessage("*nom*");
    }

    [Fact]
    public void Constructor_NormalizesEmailToLowercase()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver,
            "Paul.DURAND@Resicare.Local", "hash");

        caregiver.Email.Should().Be("paul.durand@resicare.local");
    }

    [Fact]
    public void Constructor_WithEmailWithoutAt_ThrowsDomainException()
    {
        Action act = () => new Caregiver("Paul", "Durand", CaregiverRole.Caregiver,
            "not-an-email", "hash");

        act.Should().Throw<DomainException>().WithMessage("*email*");
    }
}
