using ResiCare.Application.Assignments.Commands.AssignCaregiver;
using ResiCare.Application.Assignments.Commands.RemoveAssignment;
using ResiCare.Application.Assignments.Queries.GetAssignmentsByDate;
using ResiCare.Application.Assignments.Queries.GetMyAssignments;
using ResiCare.Application.Common.Messaging;

namespace ResiCare.Api.Endpoints;

/// <summary>Endpoints d'affectation (planning) : le responsable assigne les soignants aux résidents.</summary>
public static class AssignmentsEndpoints
{
    public static IEndpointRouteBuilder MapAssignmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assignments").WithTags("Assignments");

        // GET /api/assignments/me?date= — mes résidents du jour (soignant connecté).
        group.MapGet("/me", async (DateOnly? date, IDispatcher dispatcher, CancellationToken ct) =>
            Results.Ok(await dispatcher.Send(
                new GetMyAssignmentsQuery(date ?? DateOnly.FromDateTime(DateTime.UtcNow)), ct)));

        // GET /api/assignments?date= — planning complet du jour : réservé au responsable.
        group.MapGet("/", async (DateOnly? date, IDispatcher dispatcher, CancellationToken ct) =>
                Results.Ok(await dispatcher.Send(
                    new GetAssignmentsByDateQuery(date ?? DateOnly.FromDateTime(DateTime.UtcNow)), ct)))
            .RequireAuthorization("Manager");

        // POST /api/assignments — affecter un soignant à un résident (responsable).
        group.MapPost("/", async (AssignCaregiverCommand command, IDispatcher dispatcher, CancellationToken ct) =>
            {
                var id = await dispatcher.Send(command, ct);
                return Results.Created($"/api/assignments/{id}", new { id });
            })
            .RequireAuthorization("Manager");

        // DELETE /api/assignments/{id} — retirer une affectation (responsable).
        group.MapDelete("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.Send(new RemoveAssignmentCommand(id), ct);
                return Results.NoContent();
            })
            .RequireAuthorization("Manager");

        return app;
    }
}
