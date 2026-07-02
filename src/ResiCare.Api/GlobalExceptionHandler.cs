using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Exceptions;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Api;

/// <summary>
/// Traduit les exceptions en réponses HTTP propres et homogènes (format ProblemDetails,
/// RFC 7807). C'est ICI que les erreurs métier deviennent des 400/404/409 au lieu de 500.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
        => _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Erreur de validation"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Non autorisé"),
            NotFoundException => (StatusCodes.Status404NotFound, "Ressource introuvable"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflit"),
            DomainException => (StatusCodes.Status409Conflict, "Règle métier non respectée"),
            // Violation d'unicité / concurrence détectée à l'écriture (course après le check
            // applicatif) -> conflit (409) plutôt qu'une erreur interne (500).
            DbUpdateException => (StatusCodes.Status409Conflict, "Conflit"),
            _ => (StatusCodes.Status500InternalServerError, "Erreur interne")
        };

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception switch
            {
                ValidationException => null,
                NotFoundException or DomainException or UnauthorizedException or ConflictException => exception.Message,
                DbUpdateException => "L'opération entre en conflit avec des données existantes.",
                _ => "Une erreur inattendue est survenue." // on ne divulgue pas les détails internes
            }
        };

        // Pour une erreur de validation, on joint le détail par champ.
        if (exception is ValidationException validationException)
            problemDetails.Extensions["errors"] = validationException.Errors;

        httpContext.Response.StatusCode = status;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}
