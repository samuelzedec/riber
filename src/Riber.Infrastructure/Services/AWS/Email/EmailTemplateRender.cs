using System.Text;
using Riber.Application.Abstractions.Services.Email;

namespace Riber.Infrastructure.Services.AWS.Email;

public sealed class EmailTemplateRender : IEmailTemplateRender
{
    public async Task<string> GetTemplateAsync(string templatePath, Dictionary<string, object?> data)
    {
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Services",
            "AWS",
            "Email",
            "Templates",
            templatePath
        );

        if (!File.Exists(path))
            throw new FileNotFoundException($"Template não encontrado: {templatePath}");

        var templateContent = await File.ReadAllTextAsync(path);
        return ReplaceTemplatePlaceholders(templateContent, data);
    }

    private static string ReplaceTemplatePlaceholders(
        string templateContent, 
        Dictionary<string, object?> properties)
    {
        var sb = new StringBuilder(templateContent);
        foreach (var property in properties)
            sb.Replace($"{{{{{property.Key}}}}}", property.Value?.ToString() ?? string.Empty);

        return sb.ToString();
    }
}