using Riber.Infrastructure.Persistence.Models;
namespace Riber.Infrastructure.Tests.Persistence.Interceptors.TestModels;
/// <summary>
/// Representa um modelo de teste que estende BaseModel. Esta classe fornece propriedades específicas
/// para cenários de dados de teste da camada de infraestrutura.
/// </summary>
public class ModelTest : BaseModel
{
    public string Description { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
