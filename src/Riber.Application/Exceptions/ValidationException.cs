namespace Riber.Application.Exceptions;

public sealed class ValidationException(Dictionary<string, string[]> details) : Exception
{
    public Dictionary<string, string[]> Details => details;
}