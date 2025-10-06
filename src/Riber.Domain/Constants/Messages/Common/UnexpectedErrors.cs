namespace Riber.Domain.Constants.Messages.Common;

public static class UnexpectedErrors
{
    public const string Response = "Ocorreu um erro inesperado.";
    
    public static string LoggingTemplate()
        => "[{ClassName}] exceção inesperada: {ExceptionType} - {ExceptionMessage}";
}