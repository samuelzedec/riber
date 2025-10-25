using System.Net;

namespace Riber.Application.Exceptions;

public sealed class BadRequestException(string message) 
    : ApplicationException(message, HttpStatusCode.BadRequest);