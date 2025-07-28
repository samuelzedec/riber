namespace SnackFlow.Domain.Constants;

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
        public const string IsInvalid = "O nome não tem um formato válido";
        public static string LengthIsInvalid(byte min, byte max) 
            => $"O nome deve ter entre {min} a {max} caracteres.";
    }

    /// <summary>
    /// Contém as mensagens de erro relacionadas a propriedade FantasyName
    /// </summary>
    public static class FantasyName
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
        public const string IsNullOrEmpty = "O tipo do documento não pode ser nulo.";
    }

    /// <summary>
    /// Contém as mensagens de erro relacionadas a propriedade E-mail
    /// </summary>
    public static class Email
    {
        public const string IsNullOrEmpty = "O email não pode ser vazio.";
        public const string FormatInvalid = "O email está com o formato inválido.";
    }

    /// <summary>
    /// Contém as mensagens de erro relacionadas a propriedade Telefone
    /// </summary>
    public static class Phone
    {
        public const string IsNullOrEmpty = "O telefone não pode ser vazio.";
        public const string FormatInvalid = "O telefone está com o formato inválido.";
    }

    /// <summary>
    /// Contém mensagens de erro relacionadas a conflitos no sistema, como duplicidade de registros.
    /// </summary>
    public static class Conflict
    {
        public const string EmailAlreadyExists = "Email já cadastrado no sistema";
        public const string PhoneAlreadyExists = "Telefone já cadastrado no sistema";
        public const string TaxIdAlreadyExists = "CPF/CNPJ já cadastrado no sistema";
        public const string CorporateNameAlreadyExists = "Nome da empresa já cadastrado no sistema";
    }

    /// <summary>
    /// Contém mensagens de erro pré-definidas para cenários onde entidades ou registros não são encontrados no sistema.
    /// </summary>
    public static class NotFound
    {
        public const string Company = "Empresa não encontrada no sistema";
        public const string Permission = "Essa permissão não existe";
        public const string User = "O Usuário não foi encontrado";
    }

    /// <summary>
    /// Contém mensagens de erro relacionadas à invalidez de valores ou estados específicos.
    /// </summary>
    public static class Invalid
    {
        public const string IdIsNull = "O ID não pode ser nulo";
        public const string Casting = "Não foi possível fazer o conversão";
    }

    public static class Exception
    {
        public static string Unexpected(string name, string message) 
            => $"exception occurred: {name} - {message}";
    }
}