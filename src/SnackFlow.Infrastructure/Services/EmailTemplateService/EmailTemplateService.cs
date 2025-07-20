using Newtonsoft.Json.Linq;
using SnackFlow.Application.Abstractions.Services;

namespace SnackFlow.Infrastructure.Services.EmailTemplateService;

public sealed class EmailTemplateService : IEmailTemplateService
{
    public async Task<string> GetTemplateAsync(JObject data)
    {
        var templatePath = GetTemplatePath(data);

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template não encontrado: {templatePath}");

        var templateContent = await File.ReadAllTextAsync(templatePath);
        return ReplaceTemplatePlaceholders(templateContent, data);
    }

    private static string GetTemplatePath(JObject data)
    {
        var audience = data["audience"]?.ToString()!;
        var template = data["template"]?.ToString()!;
        
        return Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Services", "EmailTemplateService", "Templates",
            audience, template
        );
    }

    private static string ReplaceTemplatePlaceholders(string templateContent, JObject data)
    {
        foreach (var property in data.Properties())
        {
            if (property.Name is "audience" or "template")
                continue;

            var placeholder = $"{{{{{property.Name}}}}}";
            var value = property.Value.ToString();
            templateContent = templateContent.Replace(placeholder, value);
        }
        return templateContent;
    }
}