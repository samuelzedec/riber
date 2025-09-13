using Riber.Domain.Abstractions;

namespace Riber.Domain.Events;

public sealed record CompanyWelcomeEmailRequestedEvent(
    string Name,
    string Email
) : IDomainEvent;