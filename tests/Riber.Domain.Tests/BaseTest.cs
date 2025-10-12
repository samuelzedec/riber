using Bogus;

namespace Riber.Domain.Tests;

/// <summary>
/// Classe base que centraliza utilitários e configurações comuns utilizados em testes unitários.
/// </summary>
public abstract class BaseTest
{
    protected readonly Faker _faker = new();

    protected static Faker<T> CreateFaker<T>() where T : class
        => new();
}