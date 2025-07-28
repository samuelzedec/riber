using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Repositories;

namespace SnackFlow.Infrastructure.Persistence.Repositories;

public sealed class CompanyRepository(AppDbContext context)
    : BaseRepository<Company>(context), ICompanyRepository;