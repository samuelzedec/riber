namespace SnackFlow.Application.Exceptions;

public class ApplicationException(string message, int code) : Exception(message)
{
    public int Code { get; } = code;
}