namespace Riber.Application.Messages;

/// <summary>
/// Representa uma mensagem utilizada para enviar um e-mail com suporte à renderização de template.
/// </summary>
public sealed record SendEmailMessage(
    string From,
    string To,
    string Subject,
    string TemplatePath,
    Dictionary<string, object?> Model
);