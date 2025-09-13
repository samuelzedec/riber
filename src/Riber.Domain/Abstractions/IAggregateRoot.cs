namespace Riber.Domain.Abstractions;

/// <summary>
/// Interface marcadora que define uma raiz de agregação em um contexto de design orientado ao domínio (DDD).
/// Classes que implementam esta interface são consideradas raízes de agregação,
/// o que significa que elas são o ponto de entrada principal para gerenciar um conjunto de entidades relacionadas.
/// </summary>
public interface IAggregateRoot;