using System.Net;

namespace Riber.Application.Exceptions;

public abstract class ApplicationException(string message, HttpStatusCode code) : Exception(message)
{
    public HttpStatusCode Code => code;
}