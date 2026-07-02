using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Prescriptions.Commands.DiscontinuePrescription;

/// <summary>Le responsable arrête une prescription. Ne renvoie rien (Unit).</summary>
public record DiscontinuePrescriptionCommand(Guid Id) : ICommand<Unit>;
