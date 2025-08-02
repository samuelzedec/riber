namespace SnackFlow.Application.Exceptions;

public abstract class ApplicationException(string message, int code) : Exception(message)
{
    public int Code => code;
}