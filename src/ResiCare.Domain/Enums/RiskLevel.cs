namespace ResiCare.Domain.Enums;

/// <summary>
/// Niveau de risque évalué pour un résident. Correspond aux catégories produites par les
/// échelles cliniques : <b>Morse</b> (risque de chute), <b>Braden</b> (risque d'escarre), etc.
/// </summary>
public enum RiskLevel
{
    None = 1,     // Aucun / négligeable
    Low = 2,      // Faible
    Moderate = 3, // Modéré
    High = 4      // Élevé
}
