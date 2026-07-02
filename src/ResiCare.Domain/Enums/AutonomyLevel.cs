namespace ResiCare.Domain.Enums;

/// <summary>
/// Niveau d'autonomie d'un résident pour UNE activité de la vie quotidienne.
/// (Échelle simple inspirée des cotations gériatriques type AGGIR : A / B / C.)
/// </summary>
public enum AutonomyLevel
{
    Independent = 1, // Autonome
    PartialHelp = 2, // Aide partielle
    Dependent = 3    // Dépendant
}
