namespace ChefControl.Domain.SharedContext.Entities;

public abstract class Entity(Guid id) : IEquatable<Guid>
{
    #region Properties

    public Guid Id { get; } = id;

    #endregion

    #region IEquatable Implementations

    public bool Equals(Guid id)
        => Id == id;

    #endregion

    #region Overrides

    public override int GetHashCode()
        => Id.GetHashCode();

    #endregion
}