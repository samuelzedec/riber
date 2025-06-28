using MediatR;
using SnackFlow.Application.Common;

namespace SnackFlow.Application.Abstractions.Queries;

/// <summary>
/// Representa uma consulta com um tipo de resposta específico.
/// </summary>
/// <typeparam name="TResponse">
/// O tipo da resposta esperada ao executar a consulta. Este tipo deve implementar <see cref="IQueryResponse"/>.
/// </typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>> where TResponse : IQueryResponse;