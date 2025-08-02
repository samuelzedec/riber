using System.Net;

namespace SnackFlow.Application.Exceptions;

public sealed class AuthenticationServiceException(string message) 
    : ApplicationException(message, (int)HttpStatusCode.InternalServerError);