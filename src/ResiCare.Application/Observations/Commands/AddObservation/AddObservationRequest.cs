using ResiCare.Domain.Enums;

namespace ResiCare.Application.Observations.Commands.AddObservation;

/// <summary>Corps JSON du POST (le ResidentId vient de l'URL, l'auteur vient du jeton).</summary>
public record AddObservationRequest(
    ObservationCategory Category,
    string Content);
