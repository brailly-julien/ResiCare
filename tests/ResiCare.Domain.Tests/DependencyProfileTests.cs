using FluentAssertions;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;
using ResiCare.Domain.ValueObjects;

namespace ResiCare.Domain.Tests;

public class DependencyProfileTests
{
    [Fact]
    public void Constructor_WithUndefinedLevel_ThrowsDomainException()
    {
        Action act = () => new DependencyProfile(
            (AutonomyLevel)999, AutonomyLevel.Independent, AutonomyLevel.Independent,
            AutonomyLevel.Independent, AutonomyLevel.Independent);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Overall_ReturnsTheMostDependentActivity()
    {
        var profile = new DependencyProfile(
            AutonomyLevel.Independent,
            AutonomyLevel.PartialHelp,
            AutonomyLevel.Dependent, // le plus dépendant
            AutonomyLevel.Independent,
            AutonomyLevel.Independent);

        profile.Overall().Should().Be(AutonomyLevel.Dependent);
    }
}
