using SnackFlow.Domain.Abstractions;

namespace SnackFlow.Domain.Events;

public sealed record CompanyWelcomeEmailRequestedEvent(
    string Name,
    string Email
) : IDomainEvent;