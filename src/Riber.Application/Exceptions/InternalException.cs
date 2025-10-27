using System.Net;

namespace Riber.Application.Exceptions;

public sealed class InternalException(string message)
    : ApplicationException(message, HttpStatusCode.InternalServerError);
