using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Assignments.Queries.GetMyAssignments;

/// <summary>Les résidents dont le soignant connecté a la charge à une date — « mes résidents ».</summary>
public record GetMyAssignmentsQuery(DateOnly Date) : IQuery<IReadOnlyList<AssignmentResidentDto>>;
