using Riber.Domain.Entities;

namespace Riber.Domain.Repositories;

/// <summary>
/// Representa uma interface de repositório específica para gerenciamento de pedidos.
/// Estende a interface genérica IRepository, tendo Order como entidade raiz de agregado.
/// Fornece métodos para operações de persistência e acesso a dados relacionados aos pedidos.
/// </summary>
/// <remarks>
/// A interface IOrderRepository é projetada para lidar com a persistência
/// e consultas de dados diretamente associadas à raiz de agregado Order.
/// </remarks>
public interface IOrderRepository : IRepository<Order>;