namespace ResiCare.Application.Common.Messaging;

/// <summary>
/// Marqueur d'une REQUÊTE : une question qui ne modifie rien et renvoie TResponse
/// (typiquement un DTO). C'est le côté "lecture" du CQRS.
/// </summary>
public interface IQuery<out TResponse>
{
}

/// <summary>Le handler qui sait répondre à une requête donnée.</summary>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
