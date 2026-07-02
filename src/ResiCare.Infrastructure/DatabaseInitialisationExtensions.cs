using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResiCare.Application.Common.Security;
using ResiCare.Infrastructure.Persistence;

namespace ResiCare.Infrastructure;

public static class DatabaseInitialisationExtensions
{
    /// <summary>
    /// Applique les migrations en attente puis injecte les données de démo.
    /// À RÉSERVER au développement : en production, on applique les migrations comme
    /// une étape de déploiement maîtrisée (script/bundle), surtout pas au démarrage de
    /// l'appli (risques de course entre instances, schéma modifié sans contrôle).
    /// </summary>
    public static async Task InitialiseDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // SQLite (démo) : on crée le schéma directement depuis le modèle EF (les migrations sont
        // écrites pour SQL Server). SQL Server : on applique les migrations versionnées.
        if (context.Database.IsSqlite())
            await context.Database.EnsureCreatedAsync(cancellationToken);
        else
            await context.Database.MigrateAsync(cancellationToken);

        await ApplicationDbContextSeeder.SeedAsync(context, passwordHasher, cancellationToken);
    }
}
