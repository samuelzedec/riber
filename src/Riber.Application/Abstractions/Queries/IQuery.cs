using Riber.Application.Common;

namespace Riber.Application.Abstractions.Queries;

/// <summary>
/// Define uma consulta que pode ser processada para produzir um resultado.
/// </summary>
/// <typeparam name="TResponse">
/// O tipo da resposta para a consulta. Deve implementar <see cref="IQueryResponse"/>.
/// </typeparam>
public interface IQuery<TResponse> : Mediator.IQuery<Result<TResponse>> where TResponse : IQueryResponse;