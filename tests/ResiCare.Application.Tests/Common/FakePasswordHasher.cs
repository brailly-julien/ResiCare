using ResiCare.Application.Common.Security;

namespace ResiCare.Application.Tests.Common;

/// <summary>
/// Double de test pour <see cref="IPasswordHasher"/> : empreinte « H:&lt;mot de passe&gt; » et
/// compteur d'appels à <see cref="Verify"/> — pour prouver la vérification à temps constant
/// même quand le compte n'existe pas (anti-énumération).
/// </summary>
internal sealed class FakePasswordHasher : IPasswordHasher
{
    public int VerifyCallCount { get; private set; }

    public string PlaceholderHash => "H:__placeholder__";

    public string Hash(string password) => "H:" + password;

    public bool Verify(string passwordHash, string providedPassword)
    {
        VerifyCallCount++;
        return passwordHash == "H:" + providedPassword;
    }
}
