using SnackFlow.Domain.Entities;

namespace SnackFlow.Domain.Repositories;

/// <summary>
/// Representa a interface de repositório para gerenciar entidades <see cref="Company"/>.
/// Fornece métodos para manipulação de persistência, recuperação, consultas e operações
/// do ciclo de vida específicas para o agregado <see cref="Company"/>.
/// </summary>
public interface ICompanyRepository : IRepository<Company>;