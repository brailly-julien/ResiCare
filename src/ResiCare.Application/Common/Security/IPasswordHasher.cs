namespace ResiCare.Application.Common.Security;

/// <summary>
/// Hache un mot de passe et vérifie une tentative. L'algorithme (PBKDF2, bcrypt…) est un
/// détail d'Infrastructure : l'Application n'en dépend pas, elle ne voit que ce contrat.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Calcule l'empreinte à stocker (sel + dérivation inclus).</summary>
    string Hash(string password);

    /// <summary>Vérifie qu'un mot de passe en clair correspond à une empreinte stockée.</summary>
    bool Verify(string passwordHash, string providedPassword);

    /// <summary>
    /// Empreinte factice mais BIEN FORMÉE (même coût de dérivation qu'une vraie). Sert à
    /// vérifier « dans le vide » quand l'email n'existe pas, pour que la durée de réponse ne
    /// révèle pas l'existence d'un compte (anti-énumération par timing).
    /// </summary>
    string PlaceholderHash { get; }
}
