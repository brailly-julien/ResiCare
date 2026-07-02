namespace ResiCare.Application.Auth.Commands.Login;

/// <summary>Réponse d'une connexion réussie : le jeton signé, son expiration et l'utilisateur.</summary>
public record LoginResult(string Token, DateTime ExpiresAtUtc, AuthenticatedUserDto User);
