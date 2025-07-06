using SnackFlow.Domain.Entities;

namespace SnackFlow.Infrastructure.Tests.Persistence.Interceptors.TestModels;

/// <summary>
/// Representa uma entidade de teste que estende a entidade base. Esta classe fornece propriedades específicas
/// para cenários de dados de teste e herda funcionalidades comuns de entidade de domínio, como gerenciamento 
/// de identificador, timestamps de criação, atualização e exclusão.
/// </summary>
public class EntityTest(Guid id) : BaseEntity(id)
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}