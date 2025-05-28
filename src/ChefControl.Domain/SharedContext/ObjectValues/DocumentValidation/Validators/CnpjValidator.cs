using ChefControl.Domain.SharedContext.Constants;
using ChefControl.Domain.SharedContext.ObjectValues.DocumentValidation.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.DocumentValidation.Validators;

public sealed record CnpjValidator : IDocumentValidator
{
    #region Properties Private

    private const int Length = 14;

    #endregion

    #region Methods

    public void IsValid(string document)
    {
        string cnpj = Sanitize(document);
        
        if (cnpj.Length != Length)
            throw new InvalidLengthCnpjException(ErrorMessage.Cnpj.LengthIsInvalid);
            
        if (cnpj.Distinct().Count() == 1)
            throw new InvalidCnpjException(ErrorMessage.Cnpj.OnlyRepeatedDigits);
            
        int[] digits = cnpj
            .Select(c => c - '0')
            .ToArray();
        
        // Cálculo do primeiro dígito verificador
        int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int sum = 0;
        
        for (int i = 0; i < 12; i++)
            sum += digits[i] * multiplier1[i];
            
        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;
        
        if (digits[12] != digit1)
            throw new InvalidCnpjException(ErrorMessage.Cnpj.IsInvalid);
            
        // Cálculo do segundo dígito verificador
        int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        sum = 0;
        
        for (int i = 0; i < 13; i++)
            sum += digits[i] * multiplier2[i];
            
        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;
        
        if (digits[13] != digit2)
            throw new InvalidCnpjException(ErrorMessage.Cnpj.IsInvalid);
    }

    public string Sanitize(string document)
        => string.IsNullOrWhiteSpace(document) 
            ? throw new InvalidCnpjException(ErrorMessage.Cnpj.IsNullOrEmpty)
            : new string([.. document.Where(char.IsDigit)]);

    public static string Format(string document)
        => $"{document.Substring(0, 2)}.{document.Substring(2, 3)}.{document.Substring(5, 3)}/{document.Substring(8, 4)}-{document.Substring(12, 2)}";

    #endregion
}