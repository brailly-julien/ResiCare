using ResiCare.Application.Auth.Commands.Login;
using ResiCare.Application.Auth.Queries.GetMe;
using ResiCare.Application.Common.Messaging;

namespace ResiCare.Api.Endpoints;

/// <summary>Endpoints d'authentification : connexion et « qui suis-je ».</summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // POST /api/auth/login — ouvert à tous (on n'a pas encore de jeton à ce stade),
        // mais soumis à la limitation de débit (anti brute-force).
        group.MapPost("/login", async (LoginCommand command, IDispatcher dispatcher, CancellationToken ct) =>
                Results.Ok(await dispatcher.Send(command, ct)))
            .AllowAnonymous()
            .RequireRateLimiting("login");

        // GET /api/auth/me — renvoie l'identité du porteur du jeton (restauration de session).
        // Protégé par la politique par défaut (utilisateur authentifié requis).
        group.MapGet("/me", async (IDispatcher dispatcher, CancellationToken ct) =>
            Results.Ok(await dispatcher.Send(new GetMeQuery(), ct)));

        return app;
    }
}
