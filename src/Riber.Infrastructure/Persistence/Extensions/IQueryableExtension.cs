using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Riber.Domain.Specifications.Core;

namespace Riber.Infrastructure.Persistence.Extensions;

public static class IQueryableExtension
{
    /// <summary>
    /// Applies a given specification to the queryable and includes related entities based on the provided expressions.
    /// </summary>
    /// <typeparam name="T">The type of the entity being queried.</typeparam>
    /// <param name="queryable">The queryable representing the source data.</param>
    /// <param name="specification">The specification to apply to the queryable. If null, the queryable is returned as-is.</param>
    /// <param name="includes">Expressions defining related entities to be included in the query.</param>
    /// <returns>An <see cref="IQueryable&lt;T&gt;"/> that includes the applied specification and related entities as specified.</returns>
    public static IQueryable<T> GetQueryWithIncludes<T>(
        this IQueryable<T> queryable,
        Specification<T>? specification,
        params Expression<Func<T, object>>[] includes) where T : class
    {
        var query = specification is not null
            ? queryable.Where(specification)
            : queryable;

        return includes.Length > 0
            ? includes.Aggregate(query, (current, include) => current.Include(include))
            : query;
    }
}