namespace Riber.Application.Models.Email;

public abstract record BaseEmailModel(
    string To,
    string Subject,
    string TemplatePath
);