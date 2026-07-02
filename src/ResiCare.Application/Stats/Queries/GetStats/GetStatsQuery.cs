using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Stats.Queries.GetStats;

public record GetStatsQuery() : IQuery<StatsDto>;
