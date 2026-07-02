using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Caregivers.Queries.GetCaregivers;

/// <summary>Liste tous les soignants (triés par nom).</summary>
public record GetCaregiversQuery() : IQuery<IReadOnlyList<CaregiverDto>>;
