namespace Riber.Domain.Abstractions;

/// <summary>
/// Define um contrato para rastreamento das propriedades fundamentais de uma entidade, incluindo seu identificador único
/// e marcações temporais para criação, última atualização e exclusão lógica.
/// </summary>
public abstract class Tracker(Guid id)
{
    #region Properties

    public Guid Id { get; } = id;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    #endregion

    #region Methods

    public void UpdateEntity()
        => UpdatedAt = DateTime.UtcNow;

    public void DeleteEntity()
        => DeletedAt = DateTime.UtcNow;

    #endregion
}