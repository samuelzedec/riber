using ChefControl.Application.SharedContext.Results;
using MediatR;

namespace ChefControl.Application.SharedContext.Abstractions.Commands;

/// <summary>
/// Define um contrato para manipuladores de comando que processam comandos sem retorno de dados específicos.
/// </summary>
/// <remarks>
/// Esta interface estende IRequestHandler do MediatR para comandos que retornam apenas
/// um resultado de sucesso ou falha, facilitando a implementação do padrão CQRS.
/// </remarks>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

/// <summary>
/// Define um contrato para manipuladores de comando que processam comandos com retorno de dados específicos.
/// </summary>
/// <remarks>
/// Esta interface estende IRequestHandler do MediatR para comandos que retornam dados específicos
/// encapsulados em um Result, permitindo manipulação tipada de comandos no padrão CQRS.
/// </remarks>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TResponse : ICommandResponse
    where TCommand : ICommand<TResponse>;