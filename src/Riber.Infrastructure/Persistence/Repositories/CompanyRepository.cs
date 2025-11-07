using Riber.Domain.Entities;
using Riber.Domain.Entities.Company;
using Riber.Domain.Repositories;

namespace Riber.Infrastructure.Persistence.Repositories;

public sealed class CompanyRepository(AppDbContext context)
    : BaseRepository<Company>(context), ICompanyRepository;