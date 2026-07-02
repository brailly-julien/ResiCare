using ResiCare.Api.Pdf;
using ResiCare.Application.Caregivers.Queries.GetCaregivers;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Residents.Commands.ArchiveResident;
using ResiCare.Application.Residents.Commands.CreateResident;
using ResiCare.Application.Residents.Commands.UpdateResident;
using ResiCare.Application.Residents.Queries.GetResidentById;
using ResiCare.Application.Residents.Queries.GetResidents;

namespace ResiCare.Api.Endpoints;

/// <summary>
/// Endpoints REST des résidents. Chaque endpoint se contente de construire une
/// commande/requête et de la passer au dispatcher : aucune logique métier ici.
/// </summary>
public static class ResidentsEndpoints
{
    public static IEndpointRouteBuilder MapResidentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/residents").WithTags("Residents");

        // GET /api/residents?search=Dupont  -> liste / recherche
        group.MapGet("/", async (string? search, IDispatcher dispatcher, CancellationToken ct) =>
            Results.Ok(await dispatcher.Send(new GetResidentsQuery(search), ct)));

        // GET /api/residents/{id}  -> tableau de bord
        group.MapGet("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
            Results.Ok(await dispatcher.Send(new GetResidentByIdQuery(id), ct)));

        // GET /api/residents/{id}/pdf  -> fiche résident en PDF
        group.MapGet("/{id:guid}/pdf", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var dashboard = await dispatcher.Send(new GetResidentByIdQuery(id), ct);
            var caregivers = await dispatcher.Send(new GetCaregiversQuery(), ct);
            var pdf = ResidentPdfGenerator.Generate(dashboard, caregivers);
            return Results.File(pdf, "application/pdf", $"fiche-{dashboard.LastName}.pdf");
        });

        // POST /api/residents  -> création (201 + Id). Réservée au responsable.
        group.MapPost("/", async (CreateResidentCommand command, IDispatcher dispatcher, CancellationToken ct) =>
            {
                var id = await dispatcher.Send(command, ct);
                return Results.Created($"/api/residents/{id}", new { id });
            })
            .RequireAuthorization("Manager");

        // PUT /api/residents/{id}  -> modification (204). L'Id vient de l'URL, le reste du corps.
        group.MapPut("/{id:guid}", async (Guid id, UpdateResidentRequest body, IDispatcher dispatcher, CancellationToken ct) =>
        {
            await dispatcher.Send(
                new UpdateResidentCommand(
                    id, body.FirstName, body.LastName, body.RoomNumber,
                    body.Eating, body.Elimination, body.Mobility, body.Dressing, body.Hygiene,
                    body.FallRisk, body.PressureSoreRisk, body.MalnutritionRisk,
                    body.AttendingPhysician, body.EmergencyContactName, body.EmergencyContactPhone,
                    body.Occupation, body.Interests, body.Family,
                    body.ReferentCaregiverId),
                ct);
            return Results.NoContent();
        })
        .RequireAuthorization("Manager");

        // POST /api/residents/{id}/archive  -> archivage (204). Réservé au responsable.
        group.MapPost("/{id:guid}/archive", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.Send(new ArchiveResidentCommand(id), ct);
                return Results.NoContent();
            })
            .RequireAuthorization("Manager");

        return app;
    }
}
