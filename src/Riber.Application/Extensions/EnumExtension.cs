using System.ComponentModel;
using System.Reflection;

namespace Riber.Application.Extensions;

public static class EnumExtension
{
    /// <summary>
    /// Recupera a descrição do valor do enum com base no atributo <see cref="DescriptionAttribute"/>.
    /// Se nenhuma descrição estiver presente, o nome do valor do enum é retornado como string.
    /// </summary>
    /// <param name="value">O valor do enum para o qual a descrição deve ser recuperada.</param>
    /// <returns>A descrição associada ao valor do enum, ou o nome do valor do enum se nenhuma descrição for encontrada.</returns>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}