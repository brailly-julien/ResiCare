using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResiCare.Application.Common.Security;
using ResiCare.Infrastructure.Persistence;

namespace ResiCare.Api.IntegrationTests;

/// <summary>
/// Démarre l'API EN MÉMOIRE (TestServer) avec une base SQLite en mémoire à la place de
/// SQL Server. On teste ainsi les VRAIS endpoints HTTP de bout en bout, vite et isolé.
/// </summary>
public class ResiCareWebFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public ResiCareWebFactory()
    {
        // Garde la base SQLite en mémoire vivante tant que la factory existe.
        _connection.Open();

        // appsettings.json ne contient plus de secrets (ConnectionStrings:Default / Jwt:Key sont
        // vides ; fournis en dev via `dotnet user-secrets`, qui ne se charge qu'en environnement
        // Development — or les tests tournent en "Testing"). Program.cs lit ces deux valeurs de
        // façon SYNCHRONE pendant la construction du host (avant que les overrides de
        // ConfigureAppConfiguration ci-dessous ne soient fusionnés) : un simple ConfigureAppConfiguration
        // arrive trop tard pour elles. Les VARIABLES D'ENVIRONNEMENT, elles, sont lues dès
        // `WebApplication.CreateBuilder(args)` (AddEnvironmentVariables), donc visibles immédiatement.
        // Valeurs de TEST uniquement : la chaîne de connexion n'a besoin que d'être non-vide
        // (ConfigureTestServices la remplace de toute façon par SQLite juste après).
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", "unused-placeholder-overridden-by-sqlite-below");
        Environment.SetEnvironmentVariable("Jwt__Key", "integration-tests-only-signing-key-never-used-outside-ci!!");
    }

    /// <summary>Limite de tentatives de connexion (très haute par défaut pour ne pas brider les
    /// tests) ; un test dédié l'abaisse en surchargeant cette propriété.</summary>
    protected virtual string LoginPermitLimit => "1000";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Environnement "Testing" : évite l'auto-migration/seed réservés au mode Development.
        builder.UseEnvironment("Testing");

        // Limite de connexion (très haute par défaut) : les nombreux logins des tests ne doivent
        // pas être bridés. Seul le test dédié au rate-limiting abaisse ce seuil (cf. LoginPermitLimit).
        // Contrairement à ConnectionStrings:Default / Jwt:Key (cf. constructeur), cette valeur est
        // lue PAR REQUÊTE (voir Program.cs) : à ce moment le host est déjà pleinement construit,
        // donc un override via ConfigureAppConfiguration arrive largement à temps.
        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RateLimiting:LoginPermitLimit"] = LoginPermitLimit,
            }));

        builder.ConfigureTestServices(services =>
        {
            // On retire TOUTE la config EF du contexte (options + configuration du provider
            // SQL Server) avant de brancher SQLite — sinon EF voit deux providers et refuse.
            var efDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                            || d.ServiceType.Name.Contains("DbContextOptionsConfiguration"))
                .ToList();
            foreach (var descriptor in efDescriptors)
                services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
        });
    }

    /// <summary>Crée le schéma puis injecte les données de démo (idempotent).</summary>
    public async Task SeedDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await context.Database.EnsureCreatedAsync();
        await ApplicationDbContextSeeder.SeedAsync(context, passwordHasher);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}
