using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Common;

/// <summary>
/// Petites "clauses de garde" (guard clauses) réutilisables pour valider les
/// arguments à l'entrée des constructeurs/méthodes du domaine. Centraliser ces
/// vérifications évite la duplication et garde les entités lisibles.
/// 'internal' : ce helper ne sort pas de la couche Domain.
/// </summary>
internal static class Guard
{
    /// <summary>Vérifie qu'une chaîne est renseignée et la renvoie nettoyée (Trim).</summary>
    public static string AgainstNullOrWhiteSpace(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{fieldName} est obligatoire.");

        return value.Trim();
    }

    /// <summary>Vérifie qu'une valeur d'enum fait bien partie des valeurs déclarées.</summary>
    public static void AgainstUndefinedEnum<TEnum>(TEnum value, string fieldName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new DomainException($"{fieldName} a une valeur invalide : '{value}'.");
    }

    /// <summary>Vérifie qu'un identifiant n'est pas vide (Guid.Empty).</summary>
    public static void AgainstEmptyGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
            throw new DomainException($"{fieldName} est obligatoire.");
    }
}
