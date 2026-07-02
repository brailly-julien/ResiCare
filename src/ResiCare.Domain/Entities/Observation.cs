using ResiCare.Domain.Common;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Entities;

/// <summary>Une transmission / observation datée, saisie par un soignant.</summary>
public class Observation
{
    public Guid Id { get; private set; }
    public Guid ResidentId { get; private set; }
    public Guid CaregiverId { get; private set; }
    public ObservationCategory Category { get; private set; }
    public string Content { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    // Constructeur réservé à EF Core.
    private Observation()
    {
    }

    private Observation(Guid residentId, Guid caregiverId, ObservationCategory category, string content)
    {
        Content = Guard.AgainstNullOrWhiteSpace(content, "Le contenu de l'observation");
        Guard.AgainstUndefinedEnum(category, "La catégorie");
        Guard.AgainstEmptyGuid(caregiverId, "Le soignant");

        Id = Guid.CreateVersion7();
        ResidentId = residentId;
        CaregiverId = caregiverId;
        Category = category;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Crée une observation pour un résident.
    /// Méthode "factory" qui porte la règle métier : on ne peut pas ajouter
    /// d'observation à un résident archivé. On reçoit l'objet Resident (et pas
    /// seulement son Id) pour rendre cette règle explicite et facile à tester.
    /// </summary>
    public static Observation Create(
        Resident resident,
        Guid caregiverId,
        ObservationCategory category,
        string content)
    {
        ArgumentNullException.ThrowIfNull(resident);

        if (resident.IsArchived)
            throw new ResidentArchivedException(resident.Id);

        return new Observation(resident.Id, caregiverId, category, content);
    }
}
