using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities;
using Riber.Infrastructure.Extensions;

namespace Riber.Infrastructure.Persistence.Mappings.Entities;

public sealed class CompanyMap : BaseEntityConfiguration<Company>
{
    protected override string GetTableName()
        => "company";

    protected override void ConfigureEntity(EntityTypeBuilder<Company> builder)
    {
        builder
            .ConfigureTaxId("uq_company_tax_id")
            .ConfigureEmail("uq_company_email")
            .ConfigurePhone("uq_company_phone")
            .ConfigureCompanyName();
    }
}