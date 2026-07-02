using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Residents.Queries.GetResidentById;

/// <summary>Renvoie le tableau de bord complet d'un résident.</summary>
public record GetResidentByIdQuery(Guid Id) : IQuery<ResidentDashboardDto>;
