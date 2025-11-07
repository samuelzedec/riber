using Riber.Domain.Abstractions;

namespace Riber.Domain.Entities;

/// <summary>
/// Representa uma entidade base abstrata que fornece funcionalidades comuns para entidades de domínio.
/// Esta classe inclui um identificador, gerenciamento de eventos de domínio e mecanismos de comparação de igualdade.
/// </summary>
public abstract class BaseEntity(Guid id) 
    : Tracker(id), IEquatable<BaseEntity>
{
    #region Private Members

    private readonly List<IDomainEvent> _events = [];

    #endregion

    #region Overrides

    public override int GetHashCode()
        => Id.GetHashCode();

    public override bool Equals(object? obj)
        => obj is BaseEntity other && Equals(other);

    public bool Equals(BaseEntity? other)
        => other is not null
           && GetType() == other.GetType()
           && Id == other.Id;

    #endregion

    #region Domain Events

    public IReadOnlyCollection<IDomainEvent> Events()
        => _events.AsReadOnly();

    public void ClearEvents()
        => _events.Clear();

    public void RaiseEvent(IDomainEvent @event)
        => _events.Add(@event);

    #endregion

    #region Operators

    public static bool operator ==(BaseEntity? left, BaseEntity? right)
        => EqualityComparer<BaseEntity>.Default.Equals(left, right);

    public static bool operator !=(BaseEntity? left, BaseEntity? right)
        => !(left == right);

    #endregion
}