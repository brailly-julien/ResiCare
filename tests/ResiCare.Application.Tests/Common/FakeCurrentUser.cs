using ResiCare.Application.Common.Security;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.Common;

/// <summary>
/// Double de test pour <see cref="ICurrentUser"/> : simule un soignant authentifié, sans
/// avoir à monter tout le pipeline HTTP/JWT pour tester un handler.
/// </summary>
internal sealed class FakeCurrentUser : ICurrentUser
{
    public FakeCurrentUser(
        Guid? id, CaregiverRole role = CaregiverRole.Caregiver, string email = "test@resicare.local")
    {
        Id = id;
        Role = role;
        Email = email;
    }

    public Guid? Id { get; }
    public string? Email { get; }
    public CaregiverRole? Role { get; }
    public bool IsAuthenticated => Id is not null;
}
