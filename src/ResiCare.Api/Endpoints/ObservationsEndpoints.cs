using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Observations.Commands.AddObservation;
using ResiCare.Application.Observations.Queries.GetObservationsByResident;
using ResiCare.Domain.Enums;

namespace ResiCare.Api.Endpoints;

/// <summary>Endpoints des observations, imbriqués sous un résident.</summary>
public static class ObservationsEndpoints
{
    public static IEndpointRouteBuilder MapObservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/residents/{residentId:guid}/observations").WithTags("Observations");

        // GET /api/residents/{residentId}/observations?page=1&pageSize=5&category=Care
        group.MapGet("/", async (
                Guid residentId, int? page, int? pageSize, ObservationCategory? category,
                IDispatcher dispatcher, CancellationToken ct) =>
            Results.Ok(await dispatcher.Send(
                new GetObservationsByResidentQuery(residentId, page ?? 1, pageSize ?? 5, category), ct)));

        // POST /api/residents/{residentId}/observations
        group.MapPost("/", async (Guid residentId, AddObservationRequest body, IDispatcher dispatcher, CancellationToken ct) =>
        {
            // L'auteur n'est PAS dans le corps : le handler le déduit du jeton (utilisateur connecté).
            var id = await dispatcher.Send(
                new AddObservationCommand(residentId, body.Category, body.Content), ct);
            return Results.Created($"/api/residents/{residentId}/observations/{id}", new { id });
        });

        return app;
    }
}
