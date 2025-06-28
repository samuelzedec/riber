using MediatR;
using SnackFlow.Application.Common;

namespace SnackFlow.Application.Abstractions.Commands;

/// <summary>
/// Representa a interface base para um comando na camada de aplicação,
/// projetada para facilitar o uso do padrão CQRS em um contexto MediatR.
/// </summary>
/// <remarks>
/// Esta interface estende a interface IRequest do MediatR, com o comando
/// produzindo um resultado do tipo Result, que encapsula o resultado do
/// tratamento do comando, incluindo casos de sucesso e falha.
/// </remarks>
public interface ICommand : IRequest<Result>;

/// <summary>
/// Define um contrato para comandos na camada de aplicação que produzem um Result padrão.
/// </summary>
/// <remarks>
/// Esta interface facilita a implementação do padrão CQRS em conjunto com o MediatR,
/// permitindo que comandos produzam um resultado de execução uniforme encapsulando sucesso ou falha.
/// </remarks>
public interface ICommand<TCommandResponse> : IRequest<Result<TCommandResponse>> where TCommandResponse : ICommandResponse;