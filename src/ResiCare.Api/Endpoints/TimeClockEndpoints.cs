using ResiCare.Application.Common.Messaging;
using ResiCare.Application.TimeClock.Commands.ClockIn;
using ResiCare.Application.TimeClock.Commands.ClockOut;
using ResiCare.Application.TimeClock.Queries.GetMyTimeEntries;
using ResiCare.Application.TimeClock.Queries.GetTimeEntriesByDate;

namespace ResiCare.Api.Endpoints;

/// <summary>Endpoints de pointage des soignants (arrivée / départ, mes pointages, présences).</summary>
public static class TimeClockEndpoints
{
    public static IEndpointRouteBuilder MapTimeClockEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/timeclock").WithTags("TimeClock");

        // POST /api/timeclock/clock-in — le soignant connecté pointe son arrivée.
        group.MapPost("/clock-in", async (IDispatcher dispatcher, CancellationToken ct) =>
        {
            var id = await dispatcher.Send(new ClockInCommand(), ct);
            return Results.Ok(new { id });
        });

        // POST /api/timeclock/clock-out — pointe le départ (clôt le pointage ouvert).
        group.MapPost("/clock-out", async (IDispatcher dispatcher, CancellationToken ct) =>
        {
            await dispatcher.Send(new ClockOutCommand(), ct);
            return Results.NoContent();
        });

        // GET /api/timeclock/me?date=YYYY-MM-DD — mes pointages du jour (défaut : aujourd'hui).
        group.MapGet("/me", async (DateOnly? date, IDispatcher dispatcher, CancellationToken ct) =>
            Results.Ok(await dispatcher.Send(
                new GetMyTimeEntriesQuery(date ?? DateOnly.FromDateTime(DateTime.UtcNow)), ct)));

        // GET /api/timeclock?date=YYYY-MM-DD — présences du jour : réservé au responsable.
        group.MapGet("/", async (DateOnly? date, IDispatcher dispatcher, CancellationToken ct) =>
                Results.Ok(await dispatcher.Send(
                    new GetTimeEntriesByDateQuery(date ?? DateOnly.FromDateTime(DateTime.UtcNow)), ct)))
            .RequireAuthorization("Manager");

        return app;
    }
}
