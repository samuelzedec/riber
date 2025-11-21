using System.Net;

namespace Riber.Application.Exceptions;

public abstract class ApplicationException(
    string message, 
    HttpStatusCode code, 
    Dictionary<string, string[]>? details = null) 
    : Exception(message)
{
    public Dictionary<string, string[]>? Details => details;
    public HttpStatusCode Code => code;
}