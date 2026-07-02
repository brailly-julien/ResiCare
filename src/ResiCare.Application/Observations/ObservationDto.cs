using ResiCare.Domain.Enums;

namespace ResiCare.Application.Observations;

/// <summary>Vue d'une observation (lecture). Partagée par la liste et le tableau de bord.</summary>
public record ObservationDto(
    Guid Id,
    ObservationCategory Category,
    string Content,
    DateTime CreatedAt,
    Guid CaregiverId);
