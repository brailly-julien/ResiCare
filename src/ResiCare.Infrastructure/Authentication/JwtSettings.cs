namespace ResiCare.Infrastructure.Authentication;

/// <summary>
/// Paramètres du jeton JWT, liés depuis la section "Jwt" de la configuration
/// (appsettings / variables d'environnement / user-secrets).
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>Émetteur du jeton (qui l'a délivré).</summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>Destinataire prévu du jeton (pour qui il est valable).</summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>Clé secrète de signature (HMAC-SHA256 -> au moins 32 octets / 256 bits).</summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>Durée de validité du jeton, en minutes.</summary>
    public int ExpiryMinutes { get; init; } = 120;
}
