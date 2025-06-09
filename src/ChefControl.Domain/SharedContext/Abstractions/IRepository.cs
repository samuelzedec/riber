namespace ChefControl.Domain.SharedContext.Abstractions;

/// <summary>
/// Define uma interface genérica de repositório para entidades que são raiz de agregado no DDD.
/// Responsável por operações de persistência, recuperação e consultas de dados.
/// </summary>
/// <typeparam name="T">
/// Tipo da entidade raiz de agregado que deve implementar <see cref="IAggregateRoot"/>.
/// </typeparam>
public interface IRepository<T> where T : IAggregateRoot;