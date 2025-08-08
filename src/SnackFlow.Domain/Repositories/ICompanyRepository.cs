using SnackFlow.Domain.Entities;

namespace SnackFlow.Domain.Repositories;

/// <summary>
/// Interface que define um repositório para realizar operações em entidades <see cref="Company"/>.
/// Fornece métodos para manipular a persistência, recuperação e consultas específicas do domínio Company.
/// </summary>
public interface ICompanyRepository : IRepository<Company>;