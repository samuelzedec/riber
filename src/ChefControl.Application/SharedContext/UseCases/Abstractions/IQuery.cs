using ChefControl.Application.SharedContext.Results;
using MediatR;

namespace ChefControl.Application.SharedContext.UseCases.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> where TResponse : IQueryResponse;