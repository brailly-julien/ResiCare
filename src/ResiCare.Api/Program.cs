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

// Provider de base : SQL Server par défaut, SQLite pour la démo (config "Database:Provider").
var dbProvider = builder.Configuration.GetValue<string>("Database:Provider") ?? "SqlServer";
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("Default"), dbProvider);

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

// En développement OU en démo (SeedOnStartup=true) : prépare la base + injecte les données.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue("SeedOnStartup", false))
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

// En conteneur (démo), le TLS est géré par l'hébergeur en amont → pas de redirection ici.
if (app.Configuration.GetValue("EnableHttpsRedirection", true))
    app.UseHttpsRedirection();

// Sert le front Angular (fichiers publiés dans wwwroot) — MÊME ORIGINE que l'API, donc aucun
// CORS. En dev, wwwroot est vide (le front tourne sur ng serve :4200) : ces middlewares sont
// alors sans effet. Placés AVANT l'auth : les assets du SPA (dont la page de login) sont publics.
app.UseDefaultFiles();
app.UseStaticFiles();

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

// Routes CÔTÉ CLIENT d'Angular (ex. /login, /residents) : on renvoie index.html pour que le
// routeur Angular prenne le relais. AllowAnonymous, sinon la FallbackPolicy (authentifié par
// défaut) empêcherait le chargement de l'appli. Ne capture pas /api/... (déjà routé au-dessus).
app.MapFallbackToFile("index.html").AllowAnonymous();

app.Run();

// Rend la classe Program (générée par les top-level statements) accessible aux tests
// d'intégration, qui s'appuient sur WebApplicationFactory<Program>.
public partial class Program { }
