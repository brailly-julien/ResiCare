using ResiCare.Domain.Common;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Entities;

/// <summary>Une tâche de soin planifiée pour un résident à une date donnée.</summary>
public class CareTask
{
    public Guid Id { get; private set; }
    public Guid ResidentId { get; private set; }
    public string Label { get; private set; } = null!;
    public DateOnly ScheduledDate { get; private set; }
    public CareTaskStatus Status { get; private set; }
    public Guid? CompletedByCaregiverId { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Constructeur réservé à EF Core.
    private CareTask()
    {
    }

    public CareTask(Guid residentId, string label, DateOnly scheduledDate)
    {
        Label = Guard.AgainstNullOrWhiteSpace(label, "Le libellé de la tâche");
        Guard.AgainstEmptyGuid(residentId, "Le résident");

        Id = Guid.CreateVersion7();
        ResidentId = residentId;
        ScheduledDate = scheduledDate;
        Status = CareTaskStatus.Todo;
    }

    /// <summary>
    /// Marque la tâche comme réalisée. Règle métier : une tâche ne peut être
    /// complétée qu'UNE seule fois ; compléter renseigne le soignant et l'heure.
    /// </summary>
    public void Complete(Guid caregiverId)
    {
        Guard.AgainstEmptyGuid(caregiverId, "Le soignant");

        if (Status == CareTaskStatus.Done)
            throw new CareTaskAlreadyCompletedException(Id);

        Status = CareTaskStatus.Done;
        CompletedByCaregiverId = caregiverId;
        CompletedAt = DateTime.UtcNow;
    }
}
