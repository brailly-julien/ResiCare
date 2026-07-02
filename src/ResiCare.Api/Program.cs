using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using ResiCare.Api;
using ResiCare.Api.Authentication;
using ResiCare.Api.Endpoints;
using ResiCare.Application;
using ResiCare.Infrastructure;
using Scalar.AspNetCore;

// QuestPDF : licence Community (gratuite pour usage non-commercial / petite structure).
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// ----- Enregistrement des services (injection de dépendances) -----

builder.Services.AddOpenApi();

// Les enums circulent en JSON sous forme de texte ("High") plutôt que d'entier (3).
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Traduction centralisée des exceptions en réponses HTTP (ProblemDetails, RFC 7807).
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Nos deux couches, branchées via leur point d'entrée unique.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("Default")!);

// Authentification JWT + autorisation par rôles (politique "Manager") + utilisateur courant.
builder.Services.AddJwtAuthentication(builder.Configuration);

// Limitation de débit sur la connexion (anti brute-force) : N tentatives / minute et par IP.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", httpContext =>
    {
        // Lu À LA REQUÊTE (et non au démarrage) : la config est alors finalisée, donc surchargeable.
        var permitLimit = httpContext.RequestServices
            .GetRequiredService<IConfiguration>()
            .GetValue("RateLimiting:LoginPermitLimit", 5);

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1),
                PermitLimit = permitLimit,
                QueueLimit = 0
            });
    });
});

var app = builder.Build();

// En développement : applique les migrations en attente et injecte les données de démo.
if (app.Environment.IsDevelopment())
{
    await app.Services.InitialiseDatabaseAsync();
}

// ----- Pipeline HTTP -----

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Document OpenAPI (/openapi/v1.json) + UI interactive Scalar (/scalar/v1).
    // Ouverts en dev, sinon la politique par défaut exigerait un jeton pour les charger.
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference().AllowAnonymous();
}

app.UseHttpsRedirection();

// On plafonne les tentatives AVANT l'authentification (on limite avant tout travail coûteux).
app.UseRateLimiter();

// L'ORDRE compte : on authentifie (qui es-tu ?) PUIS on autorise (en as-tu le droit ?).
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapResidentEndpoints();
app.MapObservationEndpoints();
app.MapCareTaskEndpoints();
app.MapCaregiverEndpoints();
app.MapStatsEndpoints();
app.MapTimeClockEndpoints();
app.MapAssignmentEndpoints();
app.MapPrescriptionEndpoints();

app.Run();

// Rend la classe Program (générée par les top-level statements) accessible aux tests
// d'intégration, qui s'appuient sur WebApplicationFactory<Program>.
public partial class Program { }
