using Riber.Application.Common;

namespace Riber.Application.Abstractions.Commands;

/// <summary>
/// Define um contrato para manipuladores de comandos dentro da aplicação usando o padrão CQRS.
/// </summary>
/// <typeparam name="TCommand">O tipo do comando a ser manipulado.</typeparam>
/// <remarks>
/// Esta interface define a manipulação de comandos sem um tipo específico de resposta encapsulado em um Result,
/// utilizando o framework Mediator para processar comandos de forma estruturada.
/// </remarks>
public interface ICommandHandler<in TCommand> : Mediator.ICommandHandler<TCommand, Result>
    where TCommand : ICommand;

/// <summary>
/// Define um contrato para manipulação de comandos que seguem o padrão CQRS (Command Query Responsibility Segregation).
/// </summary>
/// <typeparam name="TCommand">O tipo do comando a ser manipulado.</typeparam>
/// <typeparam name="TResponse">O tipo da resposta produzida como resultado da manipulação do comando.</typeparam>
/// <remarks>
/// Esta interface estende a funcionalidade de manipulação de comandos do mediador, encapsulando respostas em um objeto
/// de resultado estruturado e promovendo segurança de tipos e modularidade no tratamento de comandos da aplicação.
/// </remarks>
public interface ICommandHandler<in TCommand, TResponse> : Mediator.ICommandHandler<TCommand, Result<TResponse>>
    where TResponse : ICommandResponse
    where TCommand : ICommand<TResponse>;