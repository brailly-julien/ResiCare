using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace ResiCare.Api.IntegrationTests;

public class PrescriptionsApiTests : IClassFixture<ResiCareWebFactory>, IAsyncLifetime
{
    private const string ManagerEmail = "marie.curie@resicare.local";
    private const string ManagerPassword = "Manager123!";
    private const string CaregiverEmail = "paul.durand@resicare.local";
    private const string CaregiverPassword = "Soignant123!";

    private readonly ResiCareWebFactory _factory;

    public PrescriptionsApiTests(ResiCareWebFactory factory) => _factory = factory;

    public Task InitializeAsync() => _factory.SeedDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPrescriptions_Authenticated_ReturnsSeeded()
    {
        var client = await CreateAuthenticatedClientAsync(CaregiverEmail, CaregiverPassword);
        var residents = await client.GetFromJsonAsync<List<ResidentSummaryResponse>>("/api/residents");
        var jeanne = residents!.First(r => r.RoomNumber == "101"); // a une prescription semée

        var prescriptions = await client.GetFromJsonAsync<List<PrescriptionResponse>>(
            $"/api/residents/{jeanne.Id}/prescriptions");

        prescriptions.Should().NotBeNull();
        prescriptions!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Prescribe_AsCaregiver_Returns403()
    {
        var client = await CreateAuthenticatedClientAsync(CaregiverEmail, CaregiverPassword);
        var residents = await client.GetFromJsonAsync<List<ResidentSummaryResponse>>("/api/residents");
        var residentId = residents!.First().Id;

        var response = await client.PostAsJsonAsync($"/api/residents/{residentId}/prescriptions", new
        {
            medicationName = "Test", dosage = "1", posology = "1x/jour", route = "Oral", startDate = "2026-06-22"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Prescribe_AsManager_ThenRecordAdministration_AsCaregiver()
    {
        var manager = await CreateAuthenticatedClientAsync(ManagerEmail, ManagerPassword);
        var residents = await manager.GetFromJsonAsync<List<ResidentSummaryResponse>>("/api/residents");
        var residentId = residents!.First().Id;

        var create = await manager.PostAsJsonAsync($"/api/residents/{residentId}/prescriptions", new
        {
            medicationName = "Amoxicilline", dosage = "500 mg", posology = "2x/jour", route = "Oral", startDate = "2026-06-22"
        });
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await create.Content.ReadFromJsonAsync<CreatedResponse>();

        // Le soignant connecté trace l'administration (l'auteur vient du jeton).
        var caregiver = await CreateAuthenticatedClientAsync(CaregiverEmail, CaregiverPassword);
        var record = await caregiver.PostAsJsonAsync(
            $"/api/prescriptions/{created!.Id}/administrations", new { status = "Given", notes = (string?)null });

        record.StatusCode.Should().Be(HttpStatusCode.Created);
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

    private sealed record PrescriptionResponse(
        Guid Id, string MedicationName, string Dosage, string Posology, string Route, bool IsDiscontinued);

    private sealed record ResidentSummaryResponse(
        Guid Id, string FirstName, string LastName, string RoomNumber,
        string OverallDependency, string FallRisk, bool IsArchived);

    private sealed record CreatedResponse(Guid Id);

    private sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, UserResponse User);

    private sealed record UserResponse(Guid Id, string FirstName, string LastName, string Email, string Role);
}
