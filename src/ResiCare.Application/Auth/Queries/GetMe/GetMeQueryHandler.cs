using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Application.Common.Messaging;
using ResiCare.Application.Common.Persistence;
using ResiCare.Application.Common.Security;

namespace ResiCare.Application.Auth.Queries.GetMe;

public sealed class GetMeQueryHandler : IQueryHandler<GetMeQuery, AuthenticatedUserDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetMeQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<AuthenticatedUserDto> Handle(GetMeQuery query, CancellationToken cancellationToken)
    {
        var id = _currentUser.Id ?? throw new UnauthorizedException("Authentification requise.");

        var caregiver = await _db.Caregivers.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new UnauthorizedException("Compte introuvable.");

        return new AuthenticatedUserDto(
            caregiver.Id, caregiver.FirstName, caregiver.LastName, caregiver.Email, caregiver.Role);
    }
}
