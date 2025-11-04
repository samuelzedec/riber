namespace Riber.Infrastructure.Persistence.Models;

/// <summary>
/// A classe BaseModel fornece uma estrutura fundamental para entidades.
/// Inclui propriedades comuns para identificar registros de forma única e
/// gerenciar seu ciclo de vida dentro do sistema.
/// </summary>
public abstract class BaseModel
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}