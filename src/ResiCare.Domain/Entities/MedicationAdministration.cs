using ResiCare.Domain.Common;
using ResiCare.Domain.Enums;

namespace ResiCare.Domain.Entities;

/// <summary>
/// Trace d'une administration (feuille de soins) : le soignant a donné le médicament, le
/// résident a refusé, ou la prise a été omise. Fabrique <see cref="Record"/> qui porte la
/// règle métier : on n'administre pas une prescription arrêtée (comme Observation sur archivé).
/// </summary>
public class MedicationAdministration
{
    public Guid Id { get; private set; }
    public Guid PrescriptionId { get; private set; }
    public Guid CaregiverId { get; private set; }
    public DateTime AdministeredAt { get; private set; }
    public AdministrationStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // Constructeur réservé à EF Core.
    private MedicationAdministration()
    {
    }

    private MedicationAdministration(Guid prescriptionId, Guid caregiverId, AdministrationStatus status, string? notes)
    {
        Guard.AgainstEmptyGuid(caregiverId, "Le soignant");
        Guard.AgainstUndefinedEnum(status, "Le statut d'administration");

        Id = Guid.CreateVersion7();
        PrescriptionId = prescriptionId;
        CaregiverId = caregiverId;
        AdministeredAt = DateTime.UtcNow;
        Status = status;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public static MedicationAdministration Record(
        Prescription prescription, Guid caregiverId, AdministrationStatus status, string? notes)
    {
        ArgumentNullException.ThrowIfNull(prescription);

        if (prescription.IsDiscontinued)
            throw new Exceptions.PrescriptionDiscontinuedException(prescription.Id);

        return new MedicationAdministration(prescription.Id, caregiverId, status, notes);
    }
}
