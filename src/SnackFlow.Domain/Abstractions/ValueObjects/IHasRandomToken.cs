using SnackFlow.Domain.ValueObjects.RandomToken;

namespace SnackFlow.Domain.Abstractions.ValueObjects;

public interface IHasRandomToken
{
    RandomToken Token { get; }
}
