using SnackFlow.Domain.Entities;

namespace SnackFlow.Domain.Repositories;

/// <summary>
/// Representa um contrato para um repositório que lida com operações de acesso a dados relacionados a usuários.
/// </summary>
public interface IUserRepository : IRepository<User>;