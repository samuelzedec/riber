using System.Net;

namespace Riber.Application.Exceptions;

public sealed class InternalException(string Message)
    : ApplicationException(Message, (int)HttpStatusCode.InternalServerError);
