using Riber.Domain.Abstractions;

namespace Riber.Infrastructure.Persistence.Models;

/// <summary>
/// A classe BaseModel fornece uma estrutura fundamental para entidades.
/// Inclui propriedades comuns para identificar registros de forma única e
/// gerenciar seu ciclo de vida dentro do sistema.
/// </summary>
public abstract class BaseModel(Guid id) : Tracker(id);