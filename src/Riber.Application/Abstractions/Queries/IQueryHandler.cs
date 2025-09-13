using Riber.Application.Common;

namespace Riber.Application.Abstractions.Queries;

/// <summary>
/// Define um contrato para manipular consultas e retornar uma resposta encapsulada em um resultado.
/// </summary>
/// <typeparam name="TQuery">O tipo da consulta que está sendo manipulada.</typeparam>
/// <typeparam name="TResponse">O tipo da resposta retornada pelo manipulador da consulta.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : Mediator.IQueryHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
    where TResponse : IQueryResponse;