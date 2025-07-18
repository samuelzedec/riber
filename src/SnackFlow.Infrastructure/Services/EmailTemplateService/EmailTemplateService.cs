using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Enums;

namespace SnackFlow.Infrastructure.Services.EmailTemplateService;

public sealed class EmailTemplateService<T>
    : IEmailTemplateService<T> where T : class
{
    public async Task<string> GetTemplateAsync(EEmailAudience audience, EEmailTemplate template, T data)
    {
        var path = $"./Templates/{audience.GetDescription()}/{template.GetDescription()}";
        if (!File.Exists(path))
            throw new FileNotFoundException($"Template não encontrado: {path}");
        
        var templateContent = await File.ReadAllTextAsync(path);
        var properties = typeof(T).GetProperties();
        
        foreach (var property in properties)
        {
            var placeholder = $"{{{{{property.Name}}}}}";
            var value = property.GetValue(data)?.ToString() ?? string.Empty;
            templateContent = templateContent.Replace(placeholder, value);
        }
        
        return templateContent;
    }

}