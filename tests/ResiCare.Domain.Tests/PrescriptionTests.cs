using FluentAssertions;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Tests;

public class PrescriptionTests
{
    private static readonly DateOnly Start = new(2026, 6, 22);

    private static Prescription Create(DateOnly? end = null) =>
        new(Guid.NewGuid(), "Paracétamol", "1000 mg", "3x/jour", MedicationRoute.Oral, Start, end, null);

    [Fact]
    public void Constructor_WithValidData_CreatesActivePrescription()
    {
        var p = Create();

        p.Id.Should().NotBe(Guid.Empty);
        p.IsActive.Should().BeTrue();
        p.IsDiscontinued.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithBlankMedication_ThrowsDomainException()
    {
        Action act = () => new Prescription(
            Guid.NewGuid(), " ", "1000 mg", "3x/jour", MedicationRoute.Oral, Start, null, null);

        act.Should().Throw<DomainException>().WithMessage("*médicament*");
    }

    [Fact]
    public void Constructor_WithEndBeforeStart_ThrowsDomainException()
    {
        Action act = () => Create(Start.AddDays(-1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Discontinue_MarksAsDiscontinued()
    {
        var p = Create();

        p.Discontinue();

        p.IsDiscontinued.Should().BeTrue();
        p.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Discontinue_CalledTwice_ThrowsDomainException()
    {
        var p = Create();
        p.Discontinue();

        Action act = () => p.Discontinue();

        act.Should().Throw<DomainException>();
    }
}
