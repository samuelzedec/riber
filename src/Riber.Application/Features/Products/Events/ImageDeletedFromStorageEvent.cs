using Riber.Application.Abstractions.Events;

namespace Riber.Application.Features.Products.Events;

public sealed record ImageDeletedFromStorageEvent(string ImageKey) 
    : IApplicationEvent;