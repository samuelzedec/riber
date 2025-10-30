namespace Riber.Application.Dtos.Email;

public abstract record BaseEmailDto(
    string To,
    string Subject,
    string TemplatePath
);