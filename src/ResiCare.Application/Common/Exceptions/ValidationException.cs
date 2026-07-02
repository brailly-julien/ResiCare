using FluentValidation.Results;

namespace ResiCare.Application.Common.Exceptions;

/// <summary>
/// Levée quand une commande échoue à la validation. Les erreurs sont regroupées par
/// champ -> l'API les renverra en 400 avec le détail, prêt à afficher côté front.
/// </summary>
public sealed class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("Une ou plusieurs erreurs de validation sont survenues.")
    {
        Errors = failures
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(f => f.ErrorMessage).ToArray());
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
