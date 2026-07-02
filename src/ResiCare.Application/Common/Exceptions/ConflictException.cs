namespace ResiCare.Application.Common.Exceptions;

/// <summary>
/// Levée pour un conflit d'ÉTAT détecté par un handler (règle inter-entités qui nécessite la
/// base) -> l'API renverra 409 Conflict. Pendant des règles purement « dans une entité », on
/// utilise plutôt une <c>DomainException</c> (également 409).
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
