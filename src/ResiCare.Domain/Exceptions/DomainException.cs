namespace ResiCare.Domain.Exceptions;

/// <summary>
/// Exception de base levée quand une règle métier est violée.
/// On distingue volontairement ces erreurs "attendues" (l'utilisateur tente
/// quelque chose d'interdit) des vrais bugs techniques. Plus tard, l'API
/// traduira ces exceptions en codes HTTP propres (400 / 409) plutôt qu'en 500.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
