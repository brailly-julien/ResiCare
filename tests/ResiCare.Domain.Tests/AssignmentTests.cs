using FluentAssertions;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Tests;

public class AssignmentTests
{
    private static readonly DateOnly Today = new(2026, 6, 22);

    [Fact]
    public void Constructor_WithValidData_CreatesAssignment()
    {
        var caregiverId = Guid.NewGuid();
        var residentId = Guid.NewGuid();

        var assignment = new Assignment(caregiverId, residentId, Today, Shift.Morning);

        assignment.Id.Should().NotBe(Guid.Empty);
        assignment.CaregiverId.Should().Be(caregiverId);
        assignment.ResidentId.Should().Be(residentId);
        assignment.Date.Should().Be(Today);
        assignment.Shift.Should().Be(Shift.Morning);
    }

    [Fact]
    public void Constructor_WithEmptyCaregiver_ThrowsDomainException()
    {
        Action act = () => new Assignment(Guid.Empty, Guid.NewGuid(), Today, Shift.Morning);

        act.Should().Throw<DomainException>().WithMessage("*soignant*");
    }

    [Fact]
    public void Constructor_WithEmptyResident_ThrowsDomainException()
    {
        Action act = () => new Assignment(Guid.NewGuid(), Guid.Empty, Today, Shift.Morning);

        act.Should().Throw<DomainException>().WithMessage("*résident*");
    }

    [Fact]
    public void Constructor_WithUndefinedShift_ThrowsDomainException()
    {
        Action act = () => new Assignment(Guid.NewGuid(), Guid.NewGuid(), Today, (Shift)99);

        act.Should().Throw<DomainException>().WithMessage("*poste*");
    }
}
