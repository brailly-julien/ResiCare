using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Assignments.Queries.GetAssignmentsByDate;

/// <summary>Toutes les affectations d'une date, avec les noms — vue planning du responsable.</summary>
public record GetAssignmentsByDateQuery(DateOnly Date) : IQuery<IReadOnlyList<AssignmentDetailDto>>;
