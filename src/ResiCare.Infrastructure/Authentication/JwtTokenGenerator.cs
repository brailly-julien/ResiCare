using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ResiCare.Application.Common.Security;
using ResiCare.Domain.Entities;

namespace ResiCare.Infrastructure.Authentication;

/// <summary>
/// Fabrique un jeton JWT signé. Le jeton transporte des "claims" (revendications) :
/// l'identifiant du soignant (sub), son email, son nom, et son RÔLE — ce dernier servira
/// à autoriser ou refuser les actions « responsable ». Signature HMAC-SHA256 avec la clé
/// secrète : le serveur peut vérifier qu'un jeton n'a pas été falsifié sans rien stocker.
/// </summary>
public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> settings) => _settings = settings.Value;

    public AuthToken Generate(Caregiver caregiver)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, caregiver.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, caregiver.Email),
            new Claim(ClaimTypes.Name, caregiver.FullName),
            new Claim(ClaimTypes.Role, caregiver.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthToken(tokenString, expiresAtUtc);
    }
}
