using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities.User;
using Riber.Infrastructure.Extensions;

namespace Riber.Infrastructure.Persistence.Mappings.Entities;

public sealed class UserMap : BaseTypeConfiguration<User>
{
    protected override string GetTableName()
        => "user";
    
    protected override void Mapping(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(u => u.CompanyId)
            .HasColumnType("uuid")
            .HasColumnName("company_id");

        builder
            .Property(u => u.Position)
            .HasColumnType("text")
            .HasColumnName("position")
            .HasConversion<string>()
            .IsRequired();

        builder
            .HasIndex(u => u.CompanyId, "ix_user_company_id");

        builder
            .HasOne(u => u.Company)
            .WithMany()
            .HasForeignKey(u => u.CompanyId)
            .HasConstraintName("fk_user_company_id")
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .Property(u => u.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("boolean")
            .IsRequired();
        
        builder
            .ConfigureTaxId("uq_user_tax_id")
            .ConfigureFullName();
    }
}