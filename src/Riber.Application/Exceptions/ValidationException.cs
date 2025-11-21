using System.Net;

namespace Riber.Application.Exceptions;

public sealed class ValidationException(Dictionary<string, string[]> details) 
    : ApplicationException("Dados Inválidos.", HttpStatusCode.BadRequest, details);