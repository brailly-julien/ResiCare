namespace ResiCare.Domain.Exceptions;

/// <summary>Levée lorsqu'on tente d'agir sur un résident archivé.</summary>
public sealed class ResidentArchivedException : DomainException
{
    public ResidentArchivedException(Guid residentId)
        : base($"Le résident '{residentId}' est archivé : aucune observation ne peut lui être ajoutée.")
    {
    }
}
