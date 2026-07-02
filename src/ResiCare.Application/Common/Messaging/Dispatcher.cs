using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using ResiCare.Application.Common.Exceptions;

// FluentValidation expose AUSSI une ValidationException : on précise qu'ici c'est la nôtre.
using ValidationException = ResiCare.Application.Common.Exceptions.ValidationException;

namespace ResiCare.Application.Common.Messaging;

/// <summary>
/// Implémentation maison du dispatcher (équivalent simplifié de MediatR).
/// Pour une commande : 1) on valide (FluentValidation), 2) on résout le handler via
/// l'injection de dépendances et on l'exécute. La résolution se fait par réflexion car
/// le type concret n'est connu qu'à l'exécution — c'est exactement ce que fait MediatR.
/// </summary>
public sealed class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _provider;

    public Dispatcher(IServiceProvider provider) => _provider = provider;

    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(command, cancellationToken);

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        dynamic handler = _provider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)command, cancellationToken);
    }

    public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        dynamic handler = _provider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)query, cancellationToken);
    }

    // Exécute tous les validateurs enregistrés pour ce type de message (s'il y en a).
    private async Task ValidateAsync(object message, CancellationToken cancellationToken)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(message.GetType());
        var validators = _provider.GetServices(validatorType).Cast<IValidator>().ToList();
        if (validators.Count == 0)
            return;

        var context = new ValidationContext<object>(message);
        var failures = new List<ValidationFailure>();

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors);
        }

        if (failures.Count > 0)
            throw new ValidationException(failures);
    }
}
