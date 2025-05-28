namespace ChefControl.Domain.SharedContext.ObjectValues.DocumentValidation;

public interface IDocumentValidator
{
    void IsValid(string document);
    string Sanitize(string document);
    static abstract string Format(string document);
}