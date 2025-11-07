using Riber.Domain.Entities;
using Riber.Domain.Entities.User;
using Riber.Domain.Repositories;

namespace Riber.Infrastructure.Persistence.Repositories;

public sealed class InvitationRepository(AppDbContext context)
    : BaseRepository<Invitation>(context), IInvitationRepository;