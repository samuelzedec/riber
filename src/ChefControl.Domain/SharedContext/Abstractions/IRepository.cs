namespace ChefControl.Domain.SharedContext.Abstractions;

public interface IRepository<T> where T : IAggregateRoot;