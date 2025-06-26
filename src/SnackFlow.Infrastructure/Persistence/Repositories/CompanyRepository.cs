using SnackFlow.Domain.CompanyContext.Entities;
using SnackFlow.Domain.CompanyContext.Repositories;

namespace SnackFlow.Infrastructure.Persistence.Repositories;

public class CompanyRepository(AppDbContext context)
    : BaseRepository<Company>(context), ICompanyRepository;