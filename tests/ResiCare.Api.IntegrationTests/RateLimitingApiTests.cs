using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace ResiCare.Api.IntegrationTests;

public class RateLimitingApiTests
{
    // Factory dédiée avec une limite basse (3) -> on déclenche le rejet de façon déterministe.
    private sealed class StrictRateLimitFactory : ResiCareWebFactory
    {
        protected override string LoginPermitLimit => "3";
    }

    [Fact]
    public async Task Login_TooManyAttempts_Returns429()
    {
        await using var factory = new StrictRateLimitFactory();
        await factory.SeedDatabaseAsync();

        var client = factory.CreateClient();
        var payload = new { email = "inconnu@resicare.local", password = "mauvais" };

        // Les 3 premières tentatives passent le limiteur (et échouent en 401 — compte inexistant).
        for (var i = 0; i < 3; i++)
        {
            var allowed = await client.PostAsJsonAsync("/api/auth/login", payload);
            allowed.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        // La 4e est rejetée par le limiteur AVANT d'atteindre l'endpoint.
        var blocked = await client.PostAsJsonAsync("/api/auth/login", payload);
        blocked.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}
