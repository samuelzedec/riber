using SnackFlow.Domain.Entities;

namespace SnackFlow.Domain.Repositories;

/// <summary>
/// Representa uma interface especializada de repositório para gerenciar entidades de usuário no domínio.
/// Fornece operações adicionais específicas para a entidade raiz de agregado <see cref="User"/>,
/// além de herdar a funcionalidade genérica de <see cref="IRepository{T}"/>.
/// </summary>
public interface IUserRepository : IRepository<User>;