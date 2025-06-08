using Bogus;

namespace ChefControl.Domain.Tests;

public abstract class BaseTest
{
    protected readonly Faker _faker = new();

    protected Faker<T> CreateFaker<T>() where T : class
        => new Faker<T>();
}