using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Common.Security;

namespace ResiCare.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResult>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        IApplicationDbContext db, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        var caregiver = await _db.Caregivers
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

        // On vérifie TOUJOURS un hash — celui du compte, ou une empreinte factice si l'email
        // n'existe pas. Ainsi la durée de réponse ne révèle pas l'existence d'un compte
        // (anti-énumération par timing). Le message d'erreur est aussi volontairement identique.
        var passwordMatches = _passwordHasher.Verify(
            caregiver?.PasswordHash ?? _passwordHasher.PlaceholderHash, command.Password);

        if (caregiver is null || !passwordMatches)
            throw new UnauthorizedException("Email ou mot de passe incorrect.");

        var token = _tokenGenerator.Generate(caregiver);

        return new LoginResult(
            token.Token,
            token.ExpiresAtUtc,
            new AuthenticatedUserDto(
                caregiver.Id, caregiver.FirstName, caregiver.LastName, caregiver.Email, caregiver.Role));
    }
}
