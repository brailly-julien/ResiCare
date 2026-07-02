using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace ResiCare.Api.IntegrationTests;

public class TimeClockApiTests : IClassFixture<ResiCareWebFactory>, IAsyncLifetime
{
    private const string ManagerEmail = "marie.curie@resicare.local";
    private const string ManagerPassword = "Manager123!";
    private const string CaregiverEmail = "paul.durand@resicare.local";
    private const string CaregiverPassword = "Soignant123!";

    private readonly ResiCareWebFactory _factory;

    public TimeClockApiTests(ResiCareWebFactory factory) => _factory = factory;

    public Task InitializeAsync() => _factory.SeedDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ClockIn_ThenMe_ReturnsOpenShift()
    {
        var client = await CreateAuthenticatedClientAsync(CaregiverEmail, CaregiverPassword);

        var clockIn = await client.PostAsync("/api/timeclock/clock-in", null);
        clockIn.StatusCode.Should().Be(HttpStatusCode.OK);

        var entries = await client.GetFromJsonAsync<List<TimeEntryResponse>>("/api/timeclock/me");
        entries.Should().NotBeNull();
        entries!.Should().Contain(e => e.ClockOutAt == null); // un pointage en cours
    }

    [Fact]
    public async Task GetPresence_AsCaregiver_Returns403()
    {
        var client = await CreateAuthenticatedClientAsync(CaregiverEmail, CaregiverPassword);

        var response = await client.GetAsync("/api/timeclock");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPresence_AsManager_ReturnsEntries()
    {
        var client = await CreateAuthenticatedClientAsync(ManagerEmail, ManagerPassword);

        // Le responsable peut aussi pointer : on garantit au moins une présence à lire.
        await client.PostAsync("/api/timeclock/clock-in", null);

        var presence = await client.GetFromJsonAsync<List<CaregiverTimeEntryResponse>>("/api/timeclock");
        presence.Should().NotBeNull();
        presence!.Should().Contain(e => e.CaregiverLastName == "Curie");
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

    private sealed record TimeEntryResponse(
        Guid Id, Guid CaregiverId, DateTime ClockInAt, DateTime? ClockOutAt, int? DurationMinutes);

    private sealed record CaregiverTimeEntryResponse(
        Guid Id, Guid CaregiverId, string CaregiverFirstName, string CaregiverLastName,
        DateTime ClockInAt, DateTime? ClockOutAt, int? DurationMinutes);

    private sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, UserResponse User);

    private sealed record UserResponse(Guid Id, string FirstName, string LastName, string Email, string Role);
}
