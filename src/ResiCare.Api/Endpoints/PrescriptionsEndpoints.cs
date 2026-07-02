using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Prescriptions.Commands.DiscontinuePrescription;
using ResiCare.Application.Prescriptions.Commands.PrescribeMedication;
using ResiCare.Application.Prescriptions.Commands.RecordAdministration;
using ResiCare.Application.Prescriptions.Queries.GetAdministrationsByResidentAndDate;
using ResiCare.Application.Prescriptions.Queries.GetPrescriptionsByResident;

namespace ResiCare.Api.Endpoints;

/// <summary>Endpoints des prescriptions et de leur administration (feuille de soins).</summary>
public static class PrescriptionsEndpoints
{
    public static IEndpointRouteBuilder MapPrescriptionEndpoints(this IEndpointRouteBuilder app)
    {
        var byResident = app.MapGroup("/api/residents/{residentId:guid}").WithTags("Prescriptions");

        // GET /api/residents/{id}/prescriptions — prescriptions d'un résident (authentifié).
        byResident.MapGet("/prescriptions", async (Guid residentId, IDispatcher dispatcher, CancellationToken ct) =>
            Results.Ok(await dispatcher.Send(new GetPrescriptionsByResidentQuery(residentId), ct)));

        // POST /api/residents/{id}/prescriptions — prescrire (responsable).
        byResident.MapPost("/prescriptions",
                async (Guid residentId, PrescribeMedicationRequest body, IDispatcher dispatcher, CancellationToken ct) =>
                {
                    var id = await dispatcher.Send(new PrescribeMedicationCommand(
                        residentId, body.MedicationName, body.Dosage, body.Posology, body.Route,
                        body.StartDate, body.EndDate, body.Instructions), ct);
                    return Results.Created($"/api/residents/{residentId}/prescriptions/{id}", new { id });
                })
            .RequireAuthorization("Manager");

        // GET /api/residents/{id}/administrations?date= — feuille de soins du jour (authentifié).
        byResident.MapGet("/administrations", async (Guid residentId, DateOnly? date, IDispatcher dispatcher, CancellationToken ct) =>
            Results.Ok(await dispatcher.Send(
                new GetAdministrationsByResidentAndDateQuery(
                    residentId, date ?? DateOnly.FromDateTime(DateTime.UtcNow)), ct)));

        var prescriptions = app.MapGroup("/api/prescriptions").WithTags("Prescriptions");

        // POST /api/prescriptions/{id}/discontinue — arrêter une prescription (responsable).
        prescriptions.MapPost("/{id:guid}/discontinue", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.Send(new DiscontinuePrescriptionCommand(id), ct);
                return Results.NoContent();
            })
            .RequireAuthorization("Manager");

        // POST /api/prescriptions/{id}/administrations — tracer une administration (soignant connecté).
        prescriptions.MapPost("/{id:guid}/administrations",
            async (Guid id, RecordAdministrationRequest body, IDispatcher dispatcher, CancellationToken ct) =>
            {
                var adminId = await dispatcher.Send(new RecordAdministrationCommand(id, body.Status, body.Notes), ct);
                return Results.Created($"/api/prescriptions/{id}/administrations/{adminId}", new { id = adminId });
            });

        return app;
    }
}
