using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackFlow.Domain.Entities;
using SnackFlow.Infrastructure.Persistence.Extensions;

namespace SnackFlow.Infrastructure.Persistence.Mappings;

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