using ResiCare.Domain.Enums;
using ResiCare.Domain.ValueObjects;

namespace ResiCare.Application.Tests.Common;

internal static class TestData
{
    public static DependencyProfile AnyDependency() =>
        new(AutonomyLevel.Independent, AutonomyLevel.Independent, AutonomyLevel.Independent,
            AutonomyLevel.Independent, AutonomyLevel.Independent);
}
