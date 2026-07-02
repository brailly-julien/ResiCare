using ResiCare.Application.Common.Security;
using ResiCare.Domain.Entities;

namespace ResiCare.Application.Tests.Common;

/// <summary>Double de test pour <see cref="IJwtTokenGenerator"/> : renvoie un jeton fixe.</summary>
internal sealed class StubJwtTokenGenerator : IJwtTokenGenerator
{
    public AuthToken Generate(Caregiver caregiver) => new("stub-token", DateTime.UtcNow.AddHours(1));
}
