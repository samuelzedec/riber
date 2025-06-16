using ChefControl.Domain.CompanyContext.Entities;
using ChefControl.Domain.CompanyContext.Repositories;

namespace ChefControl.Infrastructure.Persistence.Repositories;

public class CompanyRepository(AppDbContext context)
    : BaseRepository<Company>(context), ICompanyRepository;