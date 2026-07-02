using System.Security.Cryptography;
using ResiCare.Application.Common.Security;

namespace ResiCare.Infrastructure.Authentication;

/// <summary>
/// Hachage de mot de passe via PBKDF2 (RFC 2898) — une primitive STANDARD du framework,
/// jamais de "crypto maison". Chaque mot de passe reçoit un SEL aléatoire (empêche les
/// rainbow tables : deux mots de passe identiques ont des empreintes différentes) et
/// 100 000 itérations (ralentit fortement une attaque par force brute).
/// Format stocké : « itérations.selBase64.empreinteBase64 ».
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;   // 128 bits
    private const int KeySize = 32;    // 256 bits
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <summary>Empreinte factice (sel/clé à zéro) au format attendu : le coût de dérivation
    /// est identique à une vraie vérification, ce qui égalise la durée de réponse du login.</summary>
    public string PlaceholderHash { get; } =
        $"{Iterations}.{Convert.ToBase64String(new byte[SaltSize])}.{Convert.ToBase64String(new byte[KeySize])}";

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var subkey = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(subkey)}";
    }

    public bool Verify(string passwordHash, string providedPassword)
    {
        var parts = passwordHash.Split('.', 3);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var expectedSubkey = Convert.FromBase64String(parts[2]);

        var actualSubkey = Rfc2898DeriveBytes.Pbkdf2(
            providedPassword, salt, iterations, Algorithm, expectedSubkey.Length);

        // Comparaison à TEMPS CONSTANT : ne révèle pas, via la durée, où la comparaison a échoué.
        return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
    }
}
