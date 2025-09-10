using System.Net;

namespace SnackFlow.Application.Exceptions;

public sealed class InternalException(string Message)
    : ApplicationException(Message, (int)HttpStatusCode.InternalServerError);
