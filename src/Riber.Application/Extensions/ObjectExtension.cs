using System.Reflection;

namespace Riber.Application.Extensions;

public static class ObjectExtension
{
    /// <summary>
    /// Extrai as propriedades públicas de instância de um objeto e seus respectivos valores
    /// em um dicionário com os nomes das propriedades como chaves e seus valores correspondentes.
    /// </summary>
    /// <param name="obj">O objeto do qual as propriedades e valores serão extraídos.</param>
    /// <returns>Um dicionário contendo nomes de propriedades como chaves e seus valores correspondentes.</returns>
    public static Dictionary<string, object?> ExtractProperties(this object obj)
    {
        var properties = obj
            .GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        return properties.ToDictionary(x => x.Name.ToLowerInvariant(), x => x.GetValue(obj));
    }
}