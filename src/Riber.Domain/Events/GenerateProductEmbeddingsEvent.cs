using Riber.Domain.Abstractions;

namespace Riber.Domain.Events;

public sealed record GenerateProductEmbeddingsEvent(Guid ProductId)
    : IDomainEvent;