using System.Net;

namespace Riber.Application.Exceptions;

public sealed class AuthenticationServiceException(string message) 
    : ApplicationException(message, HttpStatusCode.InternalServerError);