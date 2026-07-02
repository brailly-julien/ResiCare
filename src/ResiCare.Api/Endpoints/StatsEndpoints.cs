using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Stats.Queries.GetStats;

namespace ResiCare.Api.Endpoints;

public static class StatsEndpoints
{
    public static IEndpointRouteBuilder MapStatsEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/stats
        app.MapGet("/api/stats", async (IDispatcher dispatcher, CancellationToken ct) =>
                Results.Ok(await dispatcher.Send(new GetStatsQuery(), ct)))
            .WithTags("Stats");

        return app;
    }
}
