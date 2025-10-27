using Riber.Domain.ValueObjects.ContentType;

namespace Riber.Domain.Abstractions.ValueObjects;

public interface IHasContentType
{
    ContentType ContentType { get; }
}