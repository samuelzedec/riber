using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Repositories;

namespace SnackFlow.Infrastructure.Persistence.Repositories;

public sealed class InvitationRepository(AppDbContext context)
    : BaseRepository<Invitation>(context), IInvitationRepository;