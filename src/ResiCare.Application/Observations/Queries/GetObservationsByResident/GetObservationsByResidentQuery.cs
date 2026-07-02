using ResiCare.Application.Common;
using ResiCare.Application.Common.Messaging;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Observations.Queries.GetObservationsByResident;

/// <summary>Observations d'un résident, paginées et filtrables par catégorie.</summary>
public record GetObservationsByResidentQuery(
    Guid ResidentId,
    int Page,
    int PageSize,
    ObservationCategory? Category) : IQuery<PagedResult<ObservationDto>>;
