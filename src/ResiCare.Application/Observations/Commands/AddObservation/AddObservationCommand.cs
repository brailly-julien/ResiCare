using ResiCare.Application.Common.Messaging;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Observations.Commands.AddObservation;

/// <summary>
/// Ajoute une observation à un résident. Renvoie l'Id de l'observation créée.
/// L'auteur (soignant) n'est PAS dans la commande : il est déduit du jeton JWT côté handler.
/// </summary>
public record AddObservationCommand(
    Guid ResidentId,
    ObservationCategory Category,
    string Content) : ICommand<Guid>;
