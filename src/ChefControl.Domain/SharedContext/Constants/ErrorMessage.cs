namespace ChefControl.Domain.SharedContext.Constants;

/// <summary>
/// Contém mensagens de erro relacionadas a validações e restrições específicas do domínio.
/// </summary>
public static class ErrorMessage
{
    /// <summary>
    /// Contém mensagens de erro relacionadas a propriedade Name.
    /// </summary>
    public static class Name
    {
        public const string IsNullOrEmpty = "O nome não pode ser vazio.";
        public static string LengthIsInvalid(byte min, byte max) 
            => $"O nome deve ter entre {min} a {max} caracteres.";
    }

    /// <summary>
    /// Contém as mensagens de erro relacionadas a propriedade TradingName
    /// </summary>
    public static class TradingName
    {
        public const string IsNullOrEmpty = "O nome fantasia não pode ser vazio.";
        public static string LengthIsInvalid(byte min, byte max) 
            => $"O nome fantasia deve ter entre {min} a {max} caracteres.";
    }

    /// <summary>
    /// Contém as mensagens de erro relacionadas a propriedade Cnpj
    /// </summary>
    public static class Cnpj
    {
        public const string IsNullOrEmpty = "O CNPJ não pode ser vazio.";
        public const string OnlyRepeatedDigits = "CNPJ não pode conter somente números iguais.";
        public const string IsInvalid = "CNPJ inválido.";
        public const string LengthIsInvalid = "CNPJ deve ter 14 dígitos para ser validado.";
    }

    /// <summary>
    /// Contém as mensagens de erro relacionadas a propriedade Cpf
    /// </summary>
    public static class Cpf
    {
        public const string IsNullOrEmpty = "O CPF não pode ser vazio.";
        public const string OnlyRepeatedDigits = "CPF não pode conter somente números iguais.";
        public const string IsInvalid = "CPF inválido.";
        public const string LengthIsInvalid = "CPF deve ter 11 dígitos para ser validado.";
    }

    /// <summary>
    /// Contém as mensagens de erro genéricas relacionadas a Documentos
    /// </summary>
    public static class Document
    {
        public const string IsInvalid = "Não é possível formatar o tipo de documento.";
    }

    /// <summary>
    /// Contém as mensagens de erro relacionadas a propriedade E-mail
    /// </summary>
    public static class Email
    {
        public const string IsNullOrEmpty = "O email não pode ser vazio.";
        public const string FormatInvalid = "O email está com o formato inválido.";
    }

    public static class Phone
    {
        public const string IsNullOrEmpty = "O telefone não pode ser vazio.";
        public const string FormatInvalid = "O telefone está com o formato inválido.";
    }
}