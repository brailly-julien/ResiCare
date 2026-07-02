using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ResiCare.Infrastructure.Persistence;

namespace ResiCare.Application.Tests.Common;

/// <summary>
/// Base des tests de handlers : fournit un ApplicationDbContext adossé à une base
/// SQLite EN MÉMOIRE (test double relationnel recommandé par Microsoft, bien plus
/// fidèle que le provider InMemory). La base vit tant que la connexion reste ouverte.
/// xUnit crée une instance de la classe par test -> chaque test a sa propre base isolée.
/// </summary>
public abstract class DatabaseTestBase : IDisposable
{
    private readonly SqliteConnection _connection;

    protected DatabaseTestBase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        using var context = CreateContext();
        context.Database.EnsureCreated(); // crée le schéma à partir du modèle EF
    }

    // Chaque appel renvoie un NOUVEAU contexte sur la MÊME base : on simule ainsi des
    // requêtes séparées (on seede avec l'un, on agit avec un autre, on vérifie avec un 3e).
    protected ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new ApplicationDbContext(options);
    }

    public void Dispose() => _connection.Dispose();
}
