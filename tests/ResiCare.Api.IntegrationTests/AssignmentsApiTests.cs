using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace ResiCare.Api.IntegrationTests;

public class AssignmentsApiTests : IClassFixture<ResiCareWebFactory>, IAsyncLifetime
{
    private const string ManagerEmail = "marie.curie@resicare.local";
    private const string ManagerPassword = "Manager123!";
    private const string CaregiverEmail = "paul.durand@resicare.local";
    private const string CaregiverPassword = "Soignant123!";

    private readonly ResiCareWebFactory _factory;

    public AssignmentsApiTests(ResiCareWebFactory factory) => _factory = factory;

    public Task InitializeAsync() => _factory.SeedDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetMine_AsCaregiver_ReturnsSeededAssignments()
    {
        var client = await CreateAuthenticatedClientAsync(CaregiverEmail, CaregiverPassword);
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");

        var mine = await client.GetFromJsonAsync<List<AssignmentResidentResponse>>($"/api/assignments/me?date={today}");

        mine.Should().NotBeNull();
        mine!.Should().NotBeEmpty(); // Paul a des résidents affectés ce jour (seed)
    }

    [Fact]
    public async Task GetPlanning_AsCaregiver_Returns403()
    {
        var client = await CreateAuthenticatedClientAsync(CaregiverEmail, CaregiverPassword);

        var response = await client.GetAsync("/api/assignments");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Assign_AsManager_Succeeds()
    {
        var client = await CreateAuthenticatedClientAsync(ManagerEmail, ManagerPassword);
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");

        var caregivers = await client.GetFromJsonAsync<List<CaregiverResponse>>("/api/caregivers");
        var residents = await client.GetFromJsonAsync<List<ResidentSummaryResponse>>("/api/residents");

        // Poste « Nuit » non utilisé par le seed -> pas de conflit d'unicité.
        var response = await client.PostAsJsonAsync("/api/assignments", new
        {
            caregiverId = caregivers!.First().Id,
            residentId = residents!.First().Id,
            date = today,
            shift = "Night"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Assign_AsCaregiver_Returns403()
    {
        var client = await CreateAuthenticatedClientAsync(CaregiverEmail, CaregiverPassword);
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");

        var response = await client.PostAsJsonAsync("/api/assignments", new
        {
            caregiverId = Guid.NewGuid(),
            residentId = Guid.NewGuid(),
            date = today,
            shift = "Morning"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync(string email, string password)
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);
        return client;
    }

    private sealed record AssignmentResidentResponse(
        Guid Id, Guid ResidentId, string ResidentFirstName, string ResidentLastName,
        string RoomNumber, DateOnly Date, string Shift);

    private sealed record CaregiverResponse(Guid Id, string FirstName, string LastName, string Role);

    private sealed record ResidentSummaryResponse(
        Guid Id, string FirstName, string LastName, string RoomNumber,
        string OverallDependency, string FallRisk, bool IsArchived);

    private sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, UserResponse User);

    private sealed record UserResponse(Guid Id, string FirstName, string LastName, string Email, string Role);
}
