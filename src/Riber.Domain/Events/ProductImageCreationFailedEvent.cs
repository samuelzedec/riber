using Riber.Domain.Abstractions;

namespace Riber.Domain.Events;

public sealed record ProductImageCreationFailedEvent(string ImageKey) 
    : IDomainEvent;