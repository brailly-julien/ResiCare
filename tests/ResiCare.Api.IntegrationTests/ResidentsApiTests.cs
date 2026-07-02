using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace ResiCare.Api.IntegrationTests;

public class ResidentsApiTests : IClassFixture<ResiCareWebFactory>, IAsyncLifetime
{
    // Comptes de démonstration injectés par le seeder.
    private const string ManagerEmail = "marie.curie@resicare.local";
    private const string ManagerPassword = "Manager123!";
    private const string CaregiverEmail = "paul.durand@resicare.local";
    private const string CaregiverPassword = "Soignant123!";

    private readonly ResiCareWebFactory _factory;

    public ResidentsApiTests(ResiCareWebFactory factory) => _factory = factory;

    public Task InitializeAsync() => _factory.SeedDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login", new { email = ManagerEmail, password = ManagerPassword });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        login!.Token.Should().NotBeNullOrWhiteSpace();
        login.User.Role.Should().Be("Manager");
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login", new { email = ManagerEmail, password = "mauvais" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetResidents_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/residents");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetResidents_Authenticated_ReturnsSeededResidents()
    {
        var client = await CreateAuthenticatedClientAsync(CaregiverEmail, CaregiverPassword);

        var residents = await client.GetFromJsonAsync<List<ResidentSummaryResponse>>("/api/residents");

        residents.Should().NotBeNull();
        residents!.Count.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task CreateResident_AsCaregiver_Returns403()
    {
        var client = await CreateAuthenticatedClientAsync(CaregiverEmail, CaregiverPassword);

        // Un soignant n'a PAS le droit de créer un résident : action réservée au responsable.
        // L'autorisation tranche avant même de lire le corps -> 403 Forbidden.
        var response = await client.PostAsJsonAsync("/api/residents", new { firstName = "Test", lastName = "Résident" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddObservation_OnArchivedResident_Returns409()
    {
        var client = await CreateAuthenticatedClientAsync(ManagerEmail, ManagerPassword);

        var residents = await client.GetFromJsonAsync<List<ResidentSummaryResponse>>("/api/residents");
        var residentId = residents!.First().Id;

        // On archive le résident (action réservée au responsable)...
        var archiveResponse = await client.PostAsync($"/api/residents/{residentId}/archive", null);
        archiveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // ...puis on tente une observation : la règle métier remonte en 409 via toute la pile HTTP.
        // L'auteur n'est plus dans le corps : il vient du jeton.
        var observationResponse = await client.PostAsJsonAsync(
            $"/api/residents/{residentId}/observations",
            new { category = "Care", content = "Test d'intégration" });

        observationResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>Se connecte et renvoie un client HTTP portant le jeton « Bearer ».</summary>
    private async Task<HttpClient> CreateAuthenticatedClientAsync(string email, string password)
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);
        return client;
    }

    // Formes de réponse minimales pour la désérialisation (l'enum est lu en texte).
    private sealed record ResidentSummaryResponse(
        Guid Id, string FirstName, string LastName, string RoomNumber,
        string OverallDependency, string FallRisk, bool IsArchived);

    private sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, UserResponse User);

    private sealed record UserResponse(Guid Id, string FirstName, string LastName, string Email, string Role);
}
