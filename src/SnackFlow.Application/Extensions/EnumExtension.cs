using System.ComponentModel;

namespace SnackFlow.Application.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        
        if(field is null)
            return value.ToString();
        
        var attribute = Attribute
            .GetCustomAttribute(field, typeof(DescriptionAttribute)) 
            as DescriptionAttribute;
        
        return attribute is null 
            ? value.ToString() 
            : attribute.Description;
    }
}