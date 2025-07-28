using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Repositories;

namespace SnackFlow.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext context)
    : BaseRepository<User>(context), IUserRepository;