namespace Riber.Application.Dtos.Email;

public sealed record WelcomeEmailDto(
    string Name,
    string To,
    string Subject,
    string TemplatePath
) : BaseEmailDto(To, Subject, TemplatePath);