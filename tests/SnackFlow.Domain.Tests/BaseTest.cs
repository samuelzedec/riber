using Bogus;

namespace SnackFlow.Domain.Tests;

/// <summary>
/// Classe base que centraliza utilitários e configurações comuns utilizados em testes unitários.
/// </summary>
public abstract class BaseTest
{
    protected readonly Faker _faker = new();

    protected Faker<T> CreateFaker<T>() where T : class
        => new();
}