using SnackFlow.Domain.SharedContext.Abstractions;

namespace SnackFlow.Domain.SharedContext.Entities;

/// <summary>
/// Representa uma entidade base abstrata que fornece funcionalidades comuns para entidades de domínio.
/// Esta classe inclui um identificador, gerenciamento de eventos de domínio e mecanismos de comparação de igualdade.
/// </summary>
public abstract class Entity(Guid id) : IEquatable<Guid>
{
    #region Private Members

    private readonly List<IDomainEvent> _events = [];
    
    #endregion
    
    #region Properties

    public Guid Id { get; } = id;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    #endregion

    #region IEquatable Implementations

    public bool Equals(Guid id)
        => Id == id;

    #endregion

    #region Overrides

    public override int GetHashCode()
        => Id.GetHashCode();

    #endregion
    
    #region Domain Events 
    
    public IReadOnlyCollection<IDomainEvent> Events()
        => _events.AsReadOnly();
    
    public void ClearEvents()
        => _events.Clear();
    
    public void RaiseEvent(IDomainEvent @event)
        => _events.Add(@event);
    
    #endregion
    
    #region Entity Methods

    public void UpdateEntity()
        => ModifiedAt = DateTime.UtcNow;
    
    public void DeleteEntity()
        => DeletedAt = DateTime.UtcNow;
    
    #endregion
}