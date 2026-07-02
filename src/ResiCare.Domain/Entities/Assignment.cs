using ResiCare.Domain.Common;
using ResiCare.Domain.Enums;

namespace ResiCare.Domain.Entities;

/// <summary>
/// Affectation : le responsable assigne un soignant à un résident pour une date et un poste
/// donnés (« qui s'occupe de quelle chambre »). Association simple mais validée à la création
/// (pas d'Id vide, poste défini). L'unicité « un soignant par résident et par poste » est
/// garantie par un index unique en base + un contrôle dans le handler (message propre).
/// </summary>
public class Assignment
{
    public Guid Id { get; private set; }
    public Guid CaregiverId { get; private set; }
    public Guid ResidentId { get; private set; }
    public DateOnly Date { get; private set; }
    public Shift Shift { get; private set; }

    // Constructeur réservé à EF Core.
    private Assignment()
    {
    }

    public Assignment(Guid caregiverId, Guid residentId, DateOnly date, Shift shift)
    {
        Guard.AgainstEmptyGuid(caregiverId, "Le soignant");
        Guard.AgainstEmptyGuid(residentId, "Le résident");
        Guard.AgainstUndefinedEnum(shift, "Le poste");

        Id = Guid.CreateVersion7();
        CaregiverId = caregiverId;
        ResidentId = residentId;
        Date = date;
        Shift = shift;
    }
}
