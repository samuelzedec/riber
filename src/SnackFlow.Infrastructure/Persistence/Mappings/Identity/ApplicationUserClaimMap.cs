using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackFlow.Infrastructure.Persistence.Identity;

namespace SnackFlow.Infrastructure.Persistence.Mappings.Identity;

public sealed class ApplicationUserClaimMap : IEntityTypeConfiguration<ApplicationUserClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
    {
        builder.ToTable("aspnet_user_claim");

        builder
            .Property(uc => uc.Id)
            .HasColumnName("id")
            .UseIdentityColumn();

        builder
            .Property(uc => uc.UserId)
            .HasColumnType("uuid")
            .HasColumnName("user_id")
            .IsRequired();

        builder
            .Property(uc => uc.ClaimType)
            .HasColumnType("text")
            .HasColumnName("claim_type")
            .IsRequired();
       
        builder
            .Property(uc => uc.ClaimValue)
            .HasColumnType("text")
            .HasColumnName("claim_value")
            .IsRequired();

        builder
            .HasIndex(uc => uc.UserId)
            .HasDatabaseName("ix_aspnet_user_claim_user_id");
    }
}