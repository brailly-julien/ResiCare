using ResiCare.Domain.Enums;

namespace ResiCare.Application.Caregivers;

/// <summary>Vue d'un soignant (lecture) — pour peupler les listes déroulantes du front.</summary>
public record CaregiverDto(Guid Id, string FirstName, string LastName, CaregiverRole Role);
