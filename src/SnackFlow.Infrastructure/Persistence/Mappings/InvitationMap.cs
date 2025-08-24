using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackFlow.Domain.Entities;
using SnackFlow.Infrastructure.Persistence.Extensions;

namespace SnackFlow.Infrastructure.Persistence.Mappings;

public sealed class InvitationMap : BaseEntityConfiguration<Invitation>
{
    protected override string GetTableName()
        => "invitations";

    protected override void ConfigureEntity(EntityTypeBuilder<Invitation> builder)
    {
        builder.ConfigureEmail("uq_invitations_email");
        
        builder
            .Property(i => i.CompanyId)
            .HasColumnName("company_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .Property(i => i.Position)
            .HasColumnName("position")
            .HasColumnType("text")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder
            .HasIndex(i => i.CompanyId, "ix_invitations_company_id");

        builder
            .Property(i => i.Role)
            .HasColumnName("role")
            .HasColumnType("text")
            .HasMaxLength(30)
            .IsRequired();

        builder
            .Property(i => i.Permissions)
            .HasColumnName("permissions")
            .HasColumnType("text")
            .IsRequired();
        
        builder
            .Property(i => i.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .Property(i => i.IsUsed)
            .HasColumnName("is_used")
            .HasColumnType("boolean")
            .IsRequired();

        builder
            .Property(i => i.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder
            .Property(i => i.InviteToken)
            .HasColumnName("invite_token")
            .HasColumnType("text")
            .HasMaxLength(64)
            .IsRequired();

        builder
            .HasIndex(i => i.InviteToken, "uq_invitations_invite_token")
            .IsUnique();
    }
    
    protected override void ConfigureQueryFilter(EntityTypeBuilder<Invitation> builder)
        => builder.HasQueryFilter(i => !i.DeletedAt.HasValue && !i.IsUsed && i.ExpiresAt > DateTime.UtcNow);
}