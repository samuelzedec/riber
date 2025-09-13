using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Persistence.Mappings.Identity;

public sealed class ApplicationRoleClaimMap : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {
        builder.ToTable("aspnet_role_claim");

        builder
            .HasKey(rc => rc.Id)
            .HasName("pk_aspnet_role_claim_id");

        builder
            .Property(rc => rc.Id)
            .HasColumnName("id")
            .UseIdentityColumn();
        
        builder
            .Property(rc => rc.RoleId)
            .HasColumnType("uuid")
            .HasColumnName("role_id")
            .IsRequired();

        builder
            .Property(rc => rc.ClaimType)
            .HasColumnType("text")
            .HasColumnName("claim_type")
            .IsRequired();
        
        builder
            .Property(rc => rc.ClaimValue)
            .HasColumnType("text")
            .HasColumnName("claim_value")
            .IsRequired();

        builder
            .HasIndex(rc => rc.RoleId)
            .HasDatabaseName("ix_aspnet_role_claim_role_id");
    }
}