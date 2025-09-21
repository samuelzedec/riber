using Riber.Domain.Entities;
using Riber.Domain.Repositories;

namespace Riber.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(AppDbContext context) 
    : BaseRepository<Order>(context), IOrderRepository;