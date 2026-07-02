namespace ResiCare.Application.Common.Exceptions;

/// <summary>Levée quand une entité référencée est introuvable -> l'API renverra 404.</summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entity, object key)
        : base($"{entity} '{key}' est introuvable.")
    {
    }
}
