using System.Text;
using Newtonsoft.Json.Linq;
using Riber.Application.Abstractions.Services.Email;

namespace Riber.Infrastructure.Services.AWS.Email;

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
        var sb = new StringBuilder(templateContent);
        foreach (var property in data.Properties())
        {
            if (property.Name is "templatePath")
                continue;

            var value = property.Value.ToString();
            sb.Replace($"{{{{{property.Name}}}}}", value);
        }
        return sb.ToString();
    }
}