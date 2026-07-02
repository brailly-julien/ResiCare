using ResiCare.Domain.Common;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;
using ResiCare.Domain.ValueObjects;

namespace ResiCare.Domain.Entities;

/// <summary>
/// Un résident de l'EMS. Modèle "riche" : setters privés, et toute modification passe
/// par une méthode qui garantit les règles métier.
/// </summary>
public class Resident
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public DateOnly BirthDate { get; private set; }
    public DateOnly AdmissionDate { get; private set; }
    public string RoomNumber { get; private set; } = null!;
    public DependencyProfile Dependency { get; private set; } = null!;
    public Guid ReferentCaregiverId { get; private set; }
    public bool IsArchived { get; private set; }

    // Risques surveillés, évalués par NIVEAU (cf. échelles Morse / Braden).
    public RiskLevel FallRisk { get; private set; }
    public RiskLevel PressureSoreRisk { get; private set; }
    public RiskLevel MalnutritionRisk { get; private set; }

    // Informations administratives (optionnelles).
    public string? AttendingPhysician { get; private set; }
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactPhone { get; private set; }

    // Mini-biographie (optionnelle) : aide à personnaliser l'accompagnement.
    public string? Occupation { get; private set; }
    public string? Interests { get; private set; }
    public string? Family { get; private set; }

    private Resident() { } // réservé à EF Core

    public Resident(
        string firstName,
        string lastName,
        DateOnly birthDate,
        DateOnly admissionDate,
        string roomNumber,
        DependencyProfile dependency,
        Guid referentCaregiverId)
    {
        FirstName = Guard.AgainstNullOrWhiteSpace(firstName, "Le prénom");
        LastName = Guard.AgainstNullOrWhiteSpace(lastName, "Le nom");
        RoomNumber = Guard.AgainstNullOrWhiteSpace(roomNumber, "Le numéro de chambre");
        Guard.AgainstEmptyGuid(referentCaregiverId, "Le soignant référent");
        ArgumentNullException.ThrowIfNull(dependency);

        if (admissionDate < birthDate)
            throw new DomainException("La date d'admission ne peut pas précéder la date de naissance.");

        Id = Guid.CreateVersion7();
        BirthDate = birthDate;
        AdmissionDate = admissionDate;
        Dependency = dependency;
        ReferentCaregiverId = referentCaregiverId;
        IsArchived = false;
        FallRisk = RiskLevel.None;
        PressureSoreRisk = RiskLevel.None;
        MalnutritionRisk = RiskLevel.None;
    }

    /// <summary>Met à jour les informations modifiables de la fiche.</summary>
    public void UpdateDetails(
        string firstName,
        string lastName,
        string roomNumber,
        DependencyProfile dependency,
        Guid referentCaregiverId)
    {
        FirstName = Guard.AgainstNullOrWhiteSpace(firstName, "Le prénom");
        LastName = Guard.AgainstNullOrWhiteSpace(lastName, "Le nom");
        RoomNumber = Guard.AgainstNullOrWhiteSpace(roomNumber, "Le numéro de chambre");
        Guard.AgainstEmptyGuid(referentCaregiverId, "Le soignant référent");
        ArgumentNullException.ThrowIfNull(dependency);

        Dependency = dependency;
        ReferentCaregiverId = referentCaregiverId;
    }

    /// <summary>Met à jour les risques surveillés.</summary>
    public void SetRisks(RiskLevel fallRisk, RiskLevel pressureSoreRisk, RiskLevel malnutritionRisk)
    {
        Guard.AgainstUndefinedEnum(fallRisk, "Le risque de chute");
        Guard.AgainstUndefinedEnum(pressureSoreRisk, "Le risque d'escarre");
        Guard.AgainstUndefinedEnum(malnutritionRisk, "Le risque de dénutrition");

        FallRisk = fallRisk;
        PressureSoreRisk = pressureSoreRisk;
        MalnutritionRisk = malnutritionRisk;
    }

    /// <summary>Met à jour les infos administratives et la mini-biographie (toutes optionnelles).</summary>
    public void SetPersonalInfo(
        string? attendingPhysician,
        string? emergencyContactName,
        string? emergencyContactPhone,
        string? occupation,
        string? interests,
        string? family)
    {
        AttendingPhysician = Normalize(attendingPhysician);
        EmergencyContactName = Normalize(emergencyContactName);
        EmergencyContactPhone = Normalize(emergencyContactPhone);
        Occupation = Normalize(occupation);
        Interests = Normalize(interests);
        Family = Normalize(family);
    }

    /// <summary>Archive le résident (soft delete). On conserve tout l'historique.</summary>
    public void Archive() => IsArchived = true;

    // "" ou "   " -> null ; sinon valeur nettoyée.
    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
