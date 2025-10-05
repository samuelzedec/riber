namespace Riber.Domain.Constants.Messages.Common;

public static class UnexpectedErrors
{
    public static string ForLogging(string className, Exception ex)
        => $"[{className}] exceção inesperada: {ex.GetType().Name} - {ex.Message}\nStack Trace: {ex.StackTrace}";
}