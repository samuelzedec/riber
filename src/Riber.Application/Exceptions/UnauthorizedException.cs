using System.Net;

namespace Riber.Application.Exceptions;

public sealed class UnauthorizedException(string message)
    : ApplicationException(message, HttpStatusCode.Unauthorized);
