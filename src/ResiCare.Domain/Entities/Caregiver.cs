using ResiCare.Domain.Common;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Entities;

/// <summary>
/// Un membre de l'équipe soignante (soignant ou responsable). C'est AUSSI l'utilisateur
/// qui se connecte : il porte donc son identifiant (email) et l'empreinte de son mot de
/// passe. Le hachage lui-même est un détail technique (couche Infrastructure) ; le Domaine
/// ne manipule jamais le mot de passe en clair, seulement le hash déjà calculé.
/// </summary>
public class Caregiver
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public CaregiverRole Role { get; private set; }

    /// <summary>Identifiant de connexion. Normalisé en minuscules pour une comparaison fiable.</summary>
    public string Email { get; private set; } = null!;

    /// <summary>Empreinte du mot de passe (jamais le mot de passe en clair).</summary>
    public string PasswordHash { get; private set; } = null!;

    // Constructeur réservé à EF Core (cf. Resident).
    private Caregiver()
    {
    }

    public Caregiver(string firstName, string lastName, CaregiverRole role, string email, string passwordHash)
    {
        FirstName = Guard.AgainstNullOrWhiteSpace(firstName, "Le prénom");
        LastName = Guard.AgainstNullOrWhiteSpace(lastName, "Le nom");
        Guard.AgainstUndefinedEnum(role, "Le rôle");

        Id = Guid.CreateVersion7();
        Role = role;
        Email = NormalizeEmail(email);
        PasswordHash = Guard.AgainstNullOrWhiteSpace(passwordHash, "L'empreinte du mot de passe");
    }

    /// <summary>Nom complet, prêt à afficher.</summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>Remplace l'empreinte du mot de passe (le hachage est fait en amont).</summary>
    public void ChangePassword(string newPasswordHash) =>
        PasswordHash = Guard.AgainstNullOrWhiteSpace(newPasswordHash, "L'empreinte du mot de passe");

    private static string NormalizeEmail(string email)
    {
        var trimmed = Guard.AgainstNullOrWhiteSpace(email, "L'email");
        if (!trimmed.Contains('@'))
            throw new DomainException("L'email est invalide.");

        return trimmed.ToLowerInvariant();
    }
}
