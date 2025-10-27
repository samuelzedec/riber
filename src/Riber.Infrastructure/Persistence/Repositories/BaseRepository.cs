using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Riber.Domain.Specifications.Core;
using Riber.Infrastructure.Extensions;

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
    
    public async Task<T?> GetSingleAsync(Specification<T> specification, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes) 
        => await Table.GetQueryWithIncludes(specification, includes).SingleOrDefaultAsync(cancellationToken);

    public async Task<bool> ExistsAsync(Specification<T> specification, CancellationToken cancellationToken = default)
        => await Table.AnyAsync(specification.ToExpression(), cancellationToken);
    
    public IQueryable<T> Query(Specification<T> specification, params Expression<Func<T, object>>[] includes)
        => Table.AsNoTracking().GetQueryWithIncludes(specification, includes);
}