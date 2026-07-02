using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Residents.Queries.GetResidents;

/// <summary>Liste des résidents, avec recherche optionnelle par nom ou prénom.</summary>
public record GetResidentsQuery(string? Search) : IQuery<IReadOnlyList<ResidentSummaryDto>>;
