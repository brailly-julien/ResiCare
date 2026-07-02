using ResiCare.Domain.Enums;

namespace ResiCare.Application.Common.Security;

/// <summary>
/// Expose le soignant AUTHENTIFIÉ pour la requête HTTP en cours. L'implémentation vit
/// dans la couche API (elle lit les "claims" du jeton JWT via HttpContext) : la couche
/// Application dépend de cette abstraction, jamais du Web. C'est ce qui permet aux handlers
/// de connaître « qui agit » sans recevoir d'identifiant depuis le client (non fiable).
/// </summary>
public interface ICurrentUser
{
    Guid? Id { get; }
    string? Email { get; }
    CaregiverRole? Role { get; }
    bool IsAuthenticated { get; }
}
