using ResiCare.Application.CareTasks.Commands.CompleteCareTask;
using ResiCare.Application.CareTasks.Commands.CreateCareTask;
using ResiCare.Application.CareTasks.Queries.GetTasksByResidentAndDate;
using ResiCare.Application.Common.Messaging;

namespace ResiCare.Api.Endpoints;

/// <summary>Endpoints des tâches de soin (planification, listing, complétion).</summary>
public static class CareTasksEndpoints
{
    public static IEndpointRouteBuilder MapCareTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var residentTasks = app.MapGroup("/api/residents/{residentId:guid}/tasks").WithTags("CareTasks");

        // GET /api/residents/{residentId}/tasks?date=2026-06-17
        residentTasks.MapGet("/", async (Guid residentId, DateOnly? date, IDispatcher dispatcher, CancellationToken ct) =>
            Results.Ok(await dispatcher.Send(new GetTasksByResidentAndDateQuery(residentId, date), ct)));

        // POST /api/residents/{residentId}/tasks  -> planification : réservée au responsable.
        residentTasks.MapPost("/", async (Guid residentId, CreateCareTaskRequest body, IDispatcher dispatcher, CancellationToken ct) =>
            {
                var id = await dispatcher.Send(new CreateCareTaskCommand(residentId, body.Label, body.ScheduledDate), ct);
                return Results.Created($"/api/residents/{residentId}/tasks/{id}", new { id });
            })
            .RequireAuthorization("Manager");

        // POST /api/tasks/{taskId}/complete  (la tâche n'est pas imbriquée sous le résident)
        // Le soignant qui valide est l'utilisateur connecté : rien dans le corps.
        var tasks = app.MapGroup("/api/tasks").WithTags("CareTasks");
        tasks.MapPost("/{taskId:guid}/complete", async (Guid taskId, IDispatcher dispatcher, CancellationToken ct) =>
        {
            await dispatcher.Send(new CompleteCareTaskCommand(taskId), ct);
            return Results.NoContent();
        });

        return app;
    }
}
