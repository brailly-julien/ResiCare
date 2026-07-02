using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Auth.Queries.GetMe;

/// <summary>« Qui suis-je ? » — relit l'identité du porteur du jeton (restauration de session).</summary>
public record GetMeQuery() : IQuery<AuthenticatedUserDto>;
