namespace ResiCare.Domain.Exceptions;

/// <summary>Levée quand on tente de clôturer un pointage déjà clos -> 409 Conflict.</summary>
public sealed class TimeEntryAlreadyClosedException : DomainException
{
    public TimeEntryAlreadyClosedException(Guid timeEntryId)
        : base($"Le pointage '{timeEntryId}' est déjà clôturé.")
    {
    }
}
