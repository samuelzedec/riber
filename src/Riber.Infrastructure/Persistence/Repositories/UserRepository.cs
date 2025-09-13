using Riber.Domain.Entities;
using Riber.Domain.Repositories;

namespace Riber.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext context)
    : BaseRepository<User>(context), IUserRepository;