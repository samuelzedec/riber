using Riber.Domain.ValueObjects.RandomToken;

namespace Riber.Domain.Abstractions.ValueObjects;

public interface IHasRandomToken
{
    RandomToken Token { get; }
}
