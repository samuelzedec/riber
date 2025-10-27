using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Queries;

namespace Riber.Application.Common;

/// <summary>
/// Representa um resultado vazio para operações que não retornam dados específicos.
/// Usado como tipo de valor para Result&lt;T&gt; quando apenas o status de sucesso/falha é relevante.
/// </summary>
public readonly struct EmptyResult : ICommandResponse, IQueryResponse;