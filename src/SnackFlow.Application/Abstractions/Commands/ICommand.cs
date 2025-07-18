using SnackFlow.Application.Common;

namespace SnackFlow.Application.Abstractions.Commands;

/// <summary>
/// Define um contrato para comandos na camada de aplicação que produzem um Result padrão.
/// </summary>
/// <remarks>
/// Esta interface é utilizada para encapsular a lógica de execução dos comandos dentro do padrão CQRS,
/// fornecendo um resultado uniforme que indica sucesso ou falha de uma operação.
/// </remarks>
public interface ICommand : Mediator.ICommand<Result>;

/// <summary>
/// Define um contrato para comandos dentro do padrão CQRS que produz um resultado padronizado
/// encapsulando o tipo de resposta.
/// </summary>
/// <typeparam name="TCommandResponse">Tipo da resposta retornada quando o comando é executado.</typeparam>
/// <remarks>
/// Esta interface serve como um contrato para comandos, permitindo que eles produzam
/// um resultado uniforme contendo o sucesso ou falha da operação junto com os respectivos dados de resposta.
/// É comumente implementada em cenários que requerem manipulação de comandos flexível e modular.
/// </remarks>
public interface ICommand<TCommandResponse> : Mediator.ICommand<Result<TCommandResponse>> where TCommandResponse : ICommandResponse;