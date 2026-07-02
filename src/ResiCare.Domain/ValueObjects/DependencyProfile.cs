using ResiCare.Domain.Common;
using ResiCare.Domain.Enums;

namespace ResiCare.Domain.ValueObjects;

/// <summary>
/// Profil de dépendance : le niveau d'autonomie du résident pour les activités clés
/// du quotidien. Inspiré des besoins fondamentaux de Virginia Henderson (le n° du
/// besoin est indiqué en commentaire). Objet-valeur IMMUABLE, validé à la construction.
/// 'record' => égalité par valeur (deux profils identiques sont "égaux").
/// </summary>
public sealed record DependencyProfile
{
    public AutonomyLevel Eating { get; private set; }       // Boire et manger (besoin 2)
    public AutonomyLevel Elimination { get; private set; }  // Éliminer — selle & urine (besoin 3)
    public AutonomyLevel Mobility { get; private set; }     // Se mouvoir / se déplacer (besoin 4)
    public AutonomyLevel Dressing { get; private set; }     // Se vêtir et se dévêtir (besoin 6)
    public AutonomyLevel Hygiene { get; private set; }      // Être propre, protéger ses téguments (besoin 8)

    private DependencyProfile() { } // réservé à EF Core

    public DependencyProfile(
        AutonomyLevel eating,
        AutonomyLevel elimination,
        AutonomyLevel mobility,
        AutonomyLevel dressing,
        AutonomyLevel hygiene)
    {
        Guard.AgainstUndefinedEnum(eating, "L'autonomie pour boire/manger");
        Guard.AgainstUndefinedEnum(elimination, "L'autonomie pour l'élimination");
        Guard.AgainstUndefinedEnum(mobility, "L'autonomie pour se mouvoir");
        Guard.AgainstUndefinedEnum(dressing, "L'autonomie pour se vêtir");
        Guard.AgainstUndefinedEnum(hygiene, "L'autonomie pour l'hygiène");

        Eating = eating;
        Elimination = elimination;
        Mobility = mobility;
        Dressing = dressing;
        Hygiene = hygiene;
    }

    /// <summary>
    /// Niveau global = le plus dépendant des activités. Méthode (et non propriété) pour
    /// qu'EF Core ne tente pas de la mapper en colonne.
    /// </summary>
    public AutonomyLevel Overall() =>
        new[] { Eating, Elimination, Mobility, Dressing, Hygiene }.Max();
}
