namespace ResiCare.Application.Common.Messaging;

/// <summary>
/// Marqueur d'une COMMANDE : une intention de modifier l'état, qui renvoie TResponse
/// (par ex. l'Id de l'entité créée). C'est le côté "écriture" du CQRS.
/// </summary>
public interface ICommand<out TResponse>
{
}

/// <summary>Le handler qui sait exécuter une commande donnée.</summary>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}
