using ResiCare.Domain.Entities;

namespace ResiCare.Application.Common.Security;

/// <summary>
/// Fabrique un jeton JWT signé pour un soignant. La construction/signature du jeton
/// (clé secrète, émetteur, durée de vie) est un détail d'Infrastructure.
/// </summary>
public interface IJwtTokenGenerator
{
    AuthToken Generate(Caregiver caregiver);
}

/// <summary>Le jeton signé et sa date d'expiration (UTC), renvoyés au client à la connexion.</summary>
public sealed record AuthToken(string Token, DateTime ExpiresAtUtc);
