using System.Net;

namespace Riber.Application.Exceptions;

public sealed class ConflictException(string message) 
    : ApplicationException(message, (int)HttpStatusCode.Conflict);