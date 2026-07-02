using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Prescriptions.Queries.GetPrescriptionsByResident;

/// <summary>Les prescriptions d'un résident (actives d'abord, puis arrêtées).</summary>
public record GetPrescriptionsByResidentQuery(Guid ResidentId) : IQuery<IReadOnlyList<PrescriptionDto>>;
