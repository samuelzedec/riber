namespace Riber.Application.DTOs.Email;

public abstract record BaseEmailDTO(
    string To,
    string Subject,
    string TemplatePath
);