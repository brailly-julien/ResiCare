using ResiCare.Domain.Enums;

namespace ResiCare.Application.Auth;

/// <summary>Identité renvoyée au front : qui suis-je, et avec quel rôle.</summary>
public record AuthenticatedUserDto(Guid Id, string FirstName, string LastName, string Email, CaregiverRole Role);
