using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Common.Security;
using ResiCare.Infrastructure.Authentication;
using ResiCare.Infrastructure.Persistence;

namespace ResiCare.Infrastructure;

/// <summary>
/// Point d'entrée unique pour brancher la couche Infrastructure dans l'injection
/// de dépendances. L'API appellera simplement services.AddInfrastructure(...).
/// Avantage : tout le détail "EF Core + SQL Server" reste enfermé ici ; l'API
/// n'a même pas besoin de connaître le provider SQL Server.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string? connectionString, string provider = "SqlServer")
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                // Utilisé pour la DÉMO déployée : conteneur unique, aucune base à héberger.
                options.UseSqlite(connectionString ?? "Data Source=resicare.db");
            }
            else
            {
                // Défaut : SQL Server (dev local via Docker, et prod « réelle »).
                ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
                options.UseSqlServer(connectionString);
            }
        });

        // Expose le DbContext derrière son abstraction, consommée par la couche Application.
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Sécurité : hachage de mot de passe + génération de jeton. Sans état -> singletons.
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
