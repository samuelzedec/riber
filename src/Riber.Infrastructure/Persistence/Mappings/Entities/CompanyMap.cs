using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities;
using Riber.Domain.Entities.Company;
using Riber.Infrastructure.Extensions;

namespace Riber.Infrastructure.Persistence.Mappings.Entities;

public sealed class CompanyMap : BaseTypeConfiguration<Company>
{
    protected override string GetTableName()
        => "company";

    protected override void Mapping(EntityTypeBuilder<Company> builder)
    {
        builder
            .ConfigureTaxId("uq_company_tax_id")
            .ConfigureEmail("uq_company_email")
            .ConfigurePhone("uq_company_phone")
            .ConfigureCompanyName();
    }
}