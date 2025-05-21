using ChefControl.Domain.Shared.ObjectValues.DocumentValidation.Exceptions;

namespace ChefControl.Domain.Shared.ObjectValues.DocumentValidation.Validators;

public sealed class CpfValidator : IDocumentValidator
{
    #region Properties Private

    private const int Length = 11;

    #endregion
    
    public void IsValid(string document)
    {
        string cpf = Sanitize(document);
        
        if (cpf.Length != Length)
            throw new InvalidLengthCpfException($"CPF deve ter {Length} dígitos para ser formatado.");
            
        if (cpf.Distinct().Count() == 1)
            throw new InvalidCpfException($"CPF não pode conter somente números iguais.");
            
        int[] digits = cpf
            .Select(c => c - '0')
            .ToArray();
        
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += digits[i] * (10 - i);
        
        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;
        
        if (digits[9] != digit1)
            throw new InvalidCpfException("CPF Inválido.");
            
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += digits[i] * (11 - i);
        
        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;
        
        if(digits[10] != digit2)
            throw new InvalidCpfException("CPF Inválido.");
    }

    public string Sanitize(string document)
        => string.IsNullOrWhiteSpace(document) 
            ? throw new InvalidCpfException("O CPF não pode ser vazio.")
            : new string([.. document.Where(char.IsDigit)]);

    public static string Format(string document)
        => $"{document.Substring(0, 3)}.{document.Substring(3, 3)}.{document.Substring(6, 3)}-{document.Substring(9, 2)}";
}