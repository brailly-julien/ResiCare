using ResiCare.Domain.Common;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Entities;

/// <summary>
/// Un pointage : l'arrivée (et plus tard le départ) d'un soignant sur son poste. Tant que
/// <see cref="ClockOutAt"/> est null, le pointage est « en cours ». La règle « on ne clôture
/// qu'une seule fois » est portée par l'entité ; la règle inter-entités « un seul pointage
/// ouvert à la fois par soignant » est portée par le handler (elle nécessite la base).
/// </summary>
public class TimeEntry
{
    public Guid Id { get; private set; }
    public Guid CaregiverId { get; private set; }
    public DateTime ClockInAt { get; private set; }
    public DateTime? ClockOutAt { get; private set; }

    // Constructeur réservé à EF Core.
    private TimeEntry()
    {
    }

    private TimeEntry(Guid caregiverId)
    {
        Guard.AgainstEmptyGuid(caregiverId, "Le soignant");

        Id = Guid.CreateVersion7();
        CaregiverId = caregiverId;
        ClockInAt = DateTime.UtcNow;
    }

    /// <summary>Le pointage est-il encore ouvert (départ non pointé) ?</summary>
    public bool IsOpen => ClockOutAt is null;

    /// <summary>Durée travaillée une fois le départ pointé (sinon null).</summary>
    public TimeSpan? Duration => ClockOutAt is null ? null : ClockOutAt.Value - ClockInAt;

    /// <summary>Pointe l'ARRIVÉE d'un soignant : ouvre un nouveau pointage.</summary>
    public static TimeEntry ClockIn(Guid caregiverId) => new(caregiverId);

    /// <summary>Pointe le DÉPART. Règle métier : on ne clôture qu'une seule fois.</summary>
    public void ClockOut()
    {
        if (!IsOpen)
            throw new TimeEntryAlreadyClosedException(Id);

        ClockOutAt = DateTime.UtcNow;
    }
}
