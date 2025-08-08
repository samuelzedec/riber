using SnackFlow.Domain.Entities;

namespace SnackFlow.Domain.Repositories;

/// <summary>
/// Define uma interface de repositório para gerenciar e persistir entidades <see cref="Invitation"/>.
/// Estende a interface genérica <see cref="IRepository{T}"/> específica para a raiz de agregado Invitation.
/// </summary>
public interface IInvitationRepository : IRepository<Invitation>;
