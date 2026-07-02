namespace ResiCare.Application.Common.Exceptions;

/// <summary>
/// Levée quand l'authentification échoue (mauvais identifiants) ou est absente alors qu'elle
/// est requise -> l'API renverra 401 Unauthorized.
/// </summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}
