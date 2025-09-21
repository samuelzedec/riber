using Riber.Domain.Specifications.Core;

namespace Riber.Infrastructure.Persistence.Extensions;

public static class SpecificationExtension
{
    /// <summary>
    /// Aplica uma especificação a uma instância IQueryable, filtrando seus elementos com base nos critérios da especificação.
    /// </summary>
    /// <param name="specification">A especificação contendo os critérios de filtragem.</param>
    /// <param name="queryable">A instância IQueryable para aplicar a especificação.</param>
    /// <typeparam name="T">O tipo dos elementos na coleção queryable.</typeparam>
    /// <returns>Uma instância IQueryable filtrada com base nos critérios da especificação.</returns>
    public static IQueryable<T> Apply<T>(this Specification<T> specification, IQueryable<T> queryable)
        => queryable.Where(specification);
}