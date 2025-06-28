namespace SnackFlow.Domain.Exceptions;

/// <summary>
/// Classe base para criação de exceções do domínio
/// </summary>
/// <param name="message">Mensagem de erro</param>
public class DomainException(string message) 
    : Exception(message);