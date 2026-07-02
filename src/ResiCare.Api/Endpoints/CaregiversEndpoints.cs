using ResiCare.Application.Caregivers.Queries.GetCaregivers;
using ResiCare.Application.Common.Messaging;

namespace ResiCare.Api.Endpoints;

public static class CaregiversEndpoints
{
    public static IEndpointRouteBuilder MapCaregiverEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/caregivers
        app.MapGet("/api/caregivers", async (IDispatcher dispatcher, CancellationToken ct) =>
                Results.Ok(await dispatcher.Send(new GetCaregiversQuery(), ct)))
            .WithTags("Caregivers");

        return app;
    }
}
