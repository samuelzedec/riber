using System.Linq.Expressions;

namespace ChefControl.Domain.SharedContext.Abstractions;

/// <summary>
/// Define uma interface genérica de repositório para entidades que são raiz de agregado no DDD.
/// Responsável por operações de persistência, recuperação e consultas de dados.
/// </summary>
/// <typeparam name="T">
/// Tipo da entidade raiz de agregado que deve implementar <see cref="IAggregateRoot"/>.
/// </typeparam>
public interface IRepository<T> where T : IAggregateRoot
{
    /// <summary>
    /// Adiciona uma nova entidade do tipo <typeparamref name="T"/> ao repositório.
    /// </summary>
    /// <param name="entity">
    /// A instância da entidade a ser adicionada.
    /// </param>
    void Create(T entity);

    /// <summary>
    /// Atualiza uma entidade existente do tipo <typeparamref name="T"/> no repositório.
    /// </summary>
    /// <param name="entity">
    /// A instância da entidade a ser atualizada.
    /// </param>
    void Update(T entity);

    /// <summary>
    /// Remove uma entidade existente do tipo <typeparamref name="T"/> do repositório.
    /// </summary>
    /// <param name="entity">
    /// A instância da entidade a ser removida.
    /// </param>
    void Delete(T entity);

    /// <summary>
    /// Recupera uma coleção consultável de entidades do tipo <typeparamref name="T"/> utilizando um predicado especificado e inclui as entidades relacionadas definidas.
    /// </summary>
    /// <param name="predicate">
    /// Uma expressão que define as condições que as entidades retornadas devem atender.
    /// </param>
    /// <param name="includes">
    /// Um array de expressões representando as entidades relacionadas a serem incluídas na consulta.
    /// </param>
    /// <returns>
    /// Uma coleção consultável de entidades do tipo <typeparamref name="T"/> que atendem ao predicado especificado.
    /// </returns>
    IQueryable<T> Query(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Recupera uma coleção consultável de entidades do tipo <typeparamref name="T"/> e inclui as entidades relacionadas especificadas.
    /// </summary>
    /// <param name="includes">
    /// Um array de expressões representando as entidades relacionadas a serem incluídas na consulta.
    /// </param>
    /// <returns>
    /// Uma coleção consultável de entidades do tipo <typeparamref name="T"/>.
    /// </returns>
    IQueryable<T> Query(params Expression<Func<T, object>>[] includes);
}