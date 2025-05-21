namespace ChefControl.Domain.Shared.ObjectValues.DocumentValidation;

public interface IDocumentValidator
{
    void IsValid(string document);
    string Sanitize(string document);
    static abstract string Format(string document);
}