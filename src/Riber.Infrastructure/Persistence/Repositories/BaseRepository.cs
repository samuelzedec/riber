using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Riber.Infrastructure.Persistence.Repositories;

/// <summary>
/// Representa uma classe base de repositório que fornece funcionalidades comuns de acesso a dados
/// para entidades no contexto de banco de dados subjacente.
/// </summary>
/// <typeparam name="T">
/// O tipo de entidade que este repositório irá manipular. Este tipo deve ser uma classe.
/// </typeparam>
public abstract class BaseRepository<T>(AppDbContext context)
    where T : class
{
    protected DbSet<T> Table { get; } = context.Set<T>();
    
    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        => await Table.AddAsync(entity, cancellationToken);
    
    public void Update(T entity)
        => Table.Update(entity);
    
    public void Delete(T entity)
        => Table.Remove(entity);
    
    public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes) 
        => await GetQueryWithIncludes(expression, includes).FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        => await Table.AnyAsync(expression, cancellationToken);
    
    public IQueryable<T> Query(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        => GetQueryWithIncludes(predicate, includes);

    /// <summary>
    /// Cria uma query na tabela do banco de dados com base em um predicado especificado e inclui as propriedades relacionadas.
    /// </summary>
    /// <param name="predicate">
    /// O predicado usado para filtrar as entidades na consulta. Esta é uma expressão que define a condição de filtragem.
    /// </param>
    /// <param name="includes">
    /// Um array de expressões que representam as propriedades relacionadas a serem incluídas na consulta.
    /// </param>
    /// <returns>
    /// Um <see cref="IQueryable{T}"/> contendo os elementos filtrados pelo predicado e incluindo as propriedades especificadas.
    /// </returns>
    private IQueryable<T> GetQueryWithIncludes(
        Expression<Func<T, bool>>? predicate,
        params Expression<Func<T, object>>[] includes)
    {
        var query = BaseQuery(predicate);
        return includes.Length > 0
            ? includes.Aggregate(query, (current, include) 
                => current.Include(include))
            : query;
    }

    /// <summary>
    /// Realiza uma consulta na tabela do banco de dados com base em um predicado especificado.
    /// </summary>
    /// <param name="predicate">
    /// O predicado usado para filtrar as entidades na consulta. Esta é uma expressão que define a condição de filtragem.
    /// </param>
    /// <returns>
    /// Um <see cref="IQueryable{T}"/> contendo os elementos que satisfazem a condição definida pelo predicado.
    /// </returns>
    private IQueryable<T> BaseQuery(Expression<Func<T, bool>>? predicate)
        => predicate is not null ? Table.Where(predicate) : Table;
}