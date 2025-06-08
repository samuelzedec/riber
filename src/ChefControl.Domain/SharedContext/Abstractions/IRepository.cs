using ChefControl.Domain.SharedContext.Abstractions;

namespace ChefControl.Domain.SharedContext.Persistence;

public interface IRepository<T> where T : IAggregateRoot;