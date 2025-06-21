using ChefControl.Application.SharedContext.Results;
using MediatR;

namespace ChefControl.Application.SharedContext.Abstractions.Queries;

/// <summary>
/// Define um manipulador para processar uma consulta e produzir um resultado.
/// </summary>
/// <typeparam name="TQuery">
/// O tipo da consulta a ser manipulada. Deve implementar <see cref="IQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// O tipo da resposta que o manipulador de consulta produz. Deve implementar <see cref="IQueryResponse"/>.
/// </typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
    where TResponse : IQueryResponse;