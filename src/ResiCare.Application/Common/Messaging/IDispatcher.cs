namespace ResiCare.Application.Common.Messaging;

/// <summary>
/// Le "chef d'orchestre" : l'API lui envoie une commande ou une requête, il trouve
/// le bon handler et l'exécute. L'API ne référence donc jamais les handlers en direct.
/// (C'est le rôle de IMediator/ISender dans MediatR.)
/// </summary>
public interface IDispatcher
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
