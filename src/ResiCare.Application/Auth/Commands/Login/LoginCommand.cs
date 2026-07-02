using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Auth.Commands.Login;

/// <summary>Tentative de connexion. Renvoie un jeton + l'identité du soignant.</summary>
public record LoginCommand(string Email, string Password) : ICommand<LoginResult>;
