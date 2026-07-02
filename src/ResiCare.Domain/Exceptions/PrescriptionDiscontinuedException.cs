namespace ResiCare.Domain.Exceptions;

/// <summary>Levée quand on tente d'administrer une prescription arrêtée -> 409 Conflict.</summary>
public sealed class PrescriptionDiscontinuedException : DomainException
{
    public PrescriptionDiscontinuedException(Guid prescriptionId)
        : base($"La prescription '{prescriptionId}' est arrêtée : administration impossible.")
    {
    }
}
