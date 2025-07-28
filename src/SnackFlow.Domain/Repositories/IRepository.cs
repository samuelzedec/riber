using System.Linq.Expressions;
using SnackFlow.Domain.Abstractions;

namespace SnackFlow.Domain.Repositories;

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
    /// Cria uma nova entidade do tipo <typeparamref name="T"/> de forma assíncrona e adiciona ao repositório.
    /// </summary>
    /// <param name="entity">
    /// A instância da entidade a ser criada.
    /// </param>
    /// <param name="cancellationToken">
    /// Token para monitorar requisições de cancelamento.
    /// </param>
    /// <returns>
    /// Uma tarefa que representa a operação assíncrona.
    /// </returns>
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

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
    /// Recupera uma única entidade do tipo <typeparamref name="T"/> que corresponde à expressão fornecida.
    /// </summary>
    /// <param name="expression">
    /// Expressão lambda utilizada para filtrar a entidade que deve ser recuperada.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelamento que pode ser utilizado para cancelar a operação.
    /// </param>
    /// <param name="includes">
    /// Expressões lambda que especificam propriedades relacionadas a serem carregadas juntamente com a entidade.
    /// </param>
    /// <returns>
    /// Uma instância da entidade do tipo <typeparamref name="T"/> ou <c>null</c> se nenhuma correspondência for encontrada.
    /// </returns>
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Verifica se existe uma entidade que atende à condição especificada.
    /// </summary>
    /// <param name="expression">
    /// Expressão lambda que define o critério da busca.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelamento que pode ser usado para cancelar a operação.
    /// </param>
    /// <returns>
    /// Retorna um valor booleano indicando se pelo menos uma entidade atende à condição especificada.
    /// </returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

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
}