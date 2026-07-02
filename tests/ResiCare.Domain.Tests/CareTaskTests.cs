using FluentAssertions;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Tests;

public class CareTaskTests
{
    [Fact]
    public void Constructor_CreatesTaskWithTodoStatus()
    {
        // Act
        var task = new CareTask(Guid.NewGuid(), "Toilette du matin", new DateOnly(2026, 6, 18));

        // Assert
        task.Status.Should().Be(CareTaskStatus.Todo);
        task.CompletedAt.Should().BeNull();
        task.CompletedByCaregiverId.Should().BeNull();
    }

    [Fact]
    public void Complete_MarksTaskDoneWithCaregiverAndTimestamp()
    {
        // Arrange
        var task = new CareTask(Guid.NewGuid(), "Toilette du matin", new DateOnly(2026, 6, 18));
        var caregiverId = Guid.NewGuid();

        // Act
        task.Complete(caregiverId);

        // Assert
        task.Status.Should().Be(CareTaskStatus.Done);
        task.CompletedByCaregiverId.Should().Be(caregiverId);
        task.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Complete_CalledTwice_ThrowsAlreadyCompletedException()
    {
        // Arrange : 2e règle métier phare. On complète une première fois...
        var task = new CareTask(Guid.NewGuid(), "Toilette du matin", new DateOnly(2026, 6, 18));
        task.Complete(Guid.NewGuid());

        // Act : ...puis une seconde fois.
        Action act = () => task.Complete(Guid.NewGuid());

        // Assert
        act.Should().Throw<CareTaskAlreadyCompletedException>();
    }
}
