using System.Security.Claims;
using ResiCare.Application.Common.Security;
using ResiCare.Domain.Enums;

namespace ResiCare.Api.Authentication;

/// <summary>
/// Implémentation de <see cref="ICurrentUser"/> côté API : lit les "claims" (revendications)
/// du jeton JWT déjà validé, qu'ASP.NET expose via <c>HttpContext.User</c>. C'est le pont
/// entre le Web (HttpContext) et la couche Application, qui ne connaît que l'abstraction.
/// On lit les noms courts ("sub", "email") ET leurs équivalents mappés, pour être robuste
/// quel que soit le réglage de mapping des claims du handler JWT.
/// </summary>
public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid? Id
    {
        get
        {
            var value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? Principal?.FindFirstValue("sub");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email =>
        Principal?.FindFirstValue(ClaimTypes.Email) ?? Principal?.FindFirstValue("email");

    public CaregiverRole? Role =>
        Enum.TryParse<CaregiverRole>(Principal?.FindFirstValue(ClaimTypes.Role), out var role) ? role : null;
}
