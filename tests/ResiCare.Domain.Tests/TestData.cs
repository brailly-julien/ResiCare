using ResiCare.Domain.Enums;
using ResiCare.Domain.ValueObjects;

namespace ResiCare.Domain.Tests;

internal static class TestData
{
    // Profil de dépendance "tout autonome" par défaut, pour les tests qui ne s'y intéressent pas.
    public static DependencyProfile AnyDependency() =>
        new(AutonomyLevel.Independent, AutonomyLevel.Independent, AutonomyLevel.Independent,
            AutonomyLevel.Independent, AutonomyLevel.Independent);
}
