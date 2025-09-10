using Newtonsoft.Json.Linq;
using SnackFlow.Application.Abstractions.Services.Email;

namespace SnackFlow.Infrastructure.Services.AWS.Email;

public sealed class EmailTemplateRender : IEmailTemplateRender
{
    public async Task<string> GetTemplateAsync(JObject data)
    {
        var templatePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Services", 
            "AWS", 
            "Email",
            "Templates",
            data["templatePath"]?.ToString()!
        );

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template não encontrado: {templatePath}");

        var templateContent = await File.ReadAllTextAsync(templatePath);
        return ReplaceTemplatePlaceholders(templateContent, data);
    }
    
    private static string ReplaceTemplatePlaceholders(string templateContent, JObject data)
    {
        foreach (var property in data.Properties())
        {
            if (property.Name is "templatePath")
                continue;

            var placeholder = $"{{{{{property.Name}}}}}";
            var value = property.Value.ToString();
            templateContent = templateContent.Replace(placeholder, value);
        }
        return templateContent;
    }
}