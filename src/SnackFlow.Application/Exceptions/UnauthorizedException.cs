using System.Net;

namespace SnackFlow.Application.Exceptions;

public sealed class UnauthorizedException(string message)
    : ApplicationException(message, (int)HttpStatusCode.Unauthorized);
