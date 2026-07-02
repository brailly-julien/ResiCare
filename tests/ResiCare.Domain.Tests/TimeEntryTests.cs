using FluentAssertions;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Tests;

public class TimeEntryTests
{
    [Fact]
    public void ClockIn_CreatesOpenEntry()
    {
        var caregiverId = Guid.NewGuid();

        var entry = TimeEntry.ClockIn(caregiverId);

        entry.Id.Should().NotBe(Guid.Empty);
        entry.CaregiverId.Should().Be(caregiverId);
        entry.IsOpen.Should().BeTrue();
        entry.ClockOutAt.Should().BeNull();
        entry.Duration.Should().BeNull();
    }

    [Fact]
    public void ClockIn_WithEmptyCaregiver_ThrowsDomainException()
    {
        Action act = () => TimeEntry.ClockIn(Guid.Empty);

        act.Should().Throw<DomainException>().WithMessage("*soignant*");
    }

    [Fact]
    public void ClockOut_ClosesEntryAndComputesDuration()
    {
        var entry = TimeEntry.ClockIn(Guid.NewGuid());

        entry.ClockOut();

        entry.IsOpen.Should().BeFalse();
        entry.ClockOutAt.Should().NotBeNull();
        entry.Duration.Should().NotBeNull();
        entry.Duration!.Value.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [Fact]
    public void ClockOut_CalledTwice_ThrowsAlreadyClosed()
    {
        var entry = TimeEntry.ClockIn(Guid.NewGuid());
        entry.ClockOut();

        Action act = () => entry.ClockOut();

        act.Should().Throw<TimeEntryAlreadyClosedException>();
    }
}
