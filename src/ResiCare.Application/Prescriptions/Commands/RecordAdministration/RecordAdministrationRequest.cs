using ResiCare.Domain.Enums;

namespace ResiCare.Application.Prescriptions.Commands.RecordAdministration;

/// <summary>Corps JSON du POST (le PrescriptionId vient de l'URL, l'auteur du jeton).</summary>
public record RecordAdministrationRequest(AdministrationStatus Status, string? Notes);
