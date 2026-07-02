using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application;

/// <summary>Point d'entrée unique pour brancher la couche Application dans l'injection
/// de dépendances : le dispatcher, tous les handlers, tous les validateurs.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Le dispatcher (notre "mini-MediatR").
        services.AddScoped<IDispatcher, Dispatcher>();

        // Enregistre automatiquement tous les handlers (ICommandHandler<,> / IQueryHandler<,>)
        // trouvés dans l'assembly : pas besoin de les déclarer un par un.
        foreach (var type in assembly.GetTypes().Where(t => t is { IsAbstract: false, IsInterface: false }))
        {
            foreach (var handlerInterface in type.GetInterfaces().Where(IsHandlerInterface))
                services.AddScoped(handlerInterface, type);
        }

        // Enregistre automatiquement tous les validateurs FluentValidation de l'assembly.
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }

    private static bool IsHandlerInterface(Type type) =>
        type.IsGenericType &&
        (type.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
         type.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
}
