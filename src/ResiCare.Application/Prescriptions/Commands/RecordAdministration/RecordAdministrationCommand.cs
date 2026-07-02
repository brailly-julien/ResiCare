using ResiCare.Application.Common.Messaging;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Prescriptions.Commands.RecordAdministration;

/// <summary>
/// Le soignant connecté trace une administration (donné / refusé / omis). Renvoie l'Id.
/// L'auteur n'est PAS dans la commande : il vient du jeton JWT (comme les observations).
/// </summary>
public record RecordAdministrationCommand(
    Guid PrescriptionId,
    AdministrationStatus Status,
    string? Notes) : ICommand<Guid>;
