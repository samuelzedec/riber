using Riber.Domain.Entities;

namespace Riber.Domain.Repositories;

/// <summary>
/// Representa uma interface de repositório específica para gerenciamento de convites.
/// Estende a interface genérica <see cref="IRepository&lt;T&gt;"/>, tendo <see cref="Invitation"/> como entidade raiz de agregado.
/// Fornece métodos para operações de persistência e acesso a dados relacionados aos convites.
/// </summary>
public interface IInvitationRepository : IRepository<Invitation>;