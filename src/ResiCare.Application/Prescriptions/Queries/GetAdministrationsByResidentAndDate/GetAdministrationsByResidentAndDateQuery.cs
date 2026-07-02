using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Prescriptions.Queries.GetAdministrationsByResidentAndDate;

/// <summary>Les administrations d'un résident pour une date (feuille de soins du jour).</summary>
public record GetAdministrationsByResidentAndDateQuery(Guid ResidentId, DateOnly Date)
    : IQuery<IReadOnlyList<AdministrationDto>>;
