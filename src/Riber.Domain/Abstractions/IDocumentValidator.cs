namespace Riber.Domain.Abstractions;

/// <summary>
/// Fornece métodos para validar, sanitizar e formatar documentos identificadores como CPF e CNPJ.
/// </summary>
public interface IDocumentValidator
{
    void IsValid(string document);
    string Sanitize(string document);
    static abstract string Format(string document);
    static string SanitizeStatic(string document)
        => new([.. document.Where(char.IsDigit)]);
    static string ValidateLength(string document, int expectedLength, Exception exception)
        => document.Length != expectedLength ? throw exception : document;
}