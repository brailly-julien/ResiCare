using FluentAssertions;
using ResiCare.Application.Auth.Commands.Login;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.Auth;

public class LoginCommandHandlerTests : DatabaseTestBase
{
    private readonly FakePasswordHasher _hasher = new();

    private async Task SeedManagerAsync()
    {
        var manager = new Caregiver("Marie", "Curie", CaregiverRole.Manager,
            "marie.curie@resicare.local", _hasher.Hash("Manager123!"));
        await using var seed = CreateContext();
        seed.Caregivers.Add(manager);
        await seed.SaveChangesAsync();
    }

    private LoginCommandHandler CreateHandler(IApplicationDbContext db) =>
        new(db, _hasher, new StubJwtTokenGenerator());

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsTokenAndUser()
    {
        await SeedManagerAsync();
        await using var ctx = CreateContext();

        var result = await CreateHandler(ctx).Handle(
            new LoginCommand("marie.curie@resicare.local", "Manager123!"), CancellationToken.None);

        result.Token.Should().Be("stub-token");
        result.User.Role.Should().Be(CaregiverRole.Manager);
        result.User.Email.Should().Be("marie.curie@resicare.local");
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsUnauthorized()
    {
        await SeedManagerAsync();
        await using var ctx = CreateContext();

        var act = async () => await CreateHandler(ctx).Handle(
            new LoginCommand("marie.curie@resicare.local", "mauvais"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_WithUnknownEmail_StillVerifiesHash_ThenThrows()
    {
        await using var ctx = CreateContext(); // base vide : aucun compte

        var act = async () => await CreateHandler(ctx).Handle(
            new LoginCommand("inconnu@resicare.local", "peu-importe"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        // Anti-timing : Verify a bien été appelé (sur l'empreinte factice) malgré l'absence de compte.
        _hasher.VerifyCallCount.Should().Be(1);
    }
}
