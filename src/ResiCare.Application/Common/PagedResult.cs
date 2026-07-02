namespace ResiCare.Application.Common;

/// <summary>Résultat paginé générique : la "page" d'éléments + le total pour l'UI.</summary>
public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
