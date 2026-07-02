namespace ResiCare.Domain.Exceptions;

/// <summary>Levée lorsqu'on tente de compléter une tâche déjà réalisée.</summary>
public sealed class CareTaskAlreadyCompletedException : DomainException
{
    public CareTaskAlreadyCompletedException(Guid careTaskId)
        : base($"La tâche de soin '{careTaskId}' est déjà marquée comme réalisée.")
    {
    }
}
