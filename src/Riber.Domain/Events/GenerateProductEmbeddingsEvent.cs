using Riber.Domain.Abstractions;
using Riber.Domain.Entities;

namespace Riber.Domain.Events;

public sealed record GenerateProductEmbeddingsEvent(Guid ProductId)
    : IDomainEvent;