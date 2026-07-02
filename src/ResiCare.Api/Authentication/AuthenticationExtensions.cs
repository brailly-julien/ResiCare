using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using ResiCare.Application.Common.Security;
using ResiCare.Domain.Enums;
using ResiCare.Infrastructure.Authentication;

namespace ResiCare.Api.Authentication;

/// <summary>
/// Branche toute la sécurité dans l'injection de dépendances : lecture des paramètres JWT,
/// validation du jeton (authentification), politiques (autorisation) et l'accès à l'utilisateur
/// courant. Regroupé ici pour garder Program.cs lisible.
/// </summary>
public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Lie la section "Jwt" -> JwtSettings (servira au générateur de jeton ET à la validation).
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        var settings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("Section de configuration 'Jwt' manquante.");

        // Permet à CurrentUser de lire HttpContext.User depuis la couche API.
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // À chaque requête, le serveur vérifie : signature (clé secrète), émetteur,
                // audience et expiration. Aucune session stockée : tout est dans le jeton signé.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1) // tolérance d'horloge réduite (défaut = 5 min)
                };
            });

        services.AddAuthorization(options =>
        {
            // SÉCURISÉ PAR DÉFAUT : sans métadonnée d'autorisation explicite, un endpoint exige
            // un utilisateur authentifié. On ouvrira explicitement les exceptions (login, docs).
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Politique "responsable" : réservée au rôle Manager.
            options.AddPolicy("Manager", policy => policy.RequireRole(nameof(CaregiverRole.Manager)));
        });

        return services;
    }
}
