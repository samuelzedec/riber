using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackFlow.Domain.Entities;

namespace SnackFlow.Infrastructure.Persistence.Mappings;

public sealed class InvitationMap : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.ToTable("invitations");

        builder
            .HasKey(i => i.Id)
            .HasName("pk_invitations_id");

        builder
            .Property(i => i.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.OwnsOne(c => c.Email, invitation =>
        {
            invitation
                .Property(c => c.Value)
                .HasColumnName("email")
                .HasColumnType("text")
                .HasMaxLength(255)
                .IsRequired();

            invitation
                .HasIndex(c => c.Value, "uq_company_email")
                .IsUnique();
        });

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

        builder
            .Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder
            .Property(c => c.UpdatedAt)
            .HasColumnName("modified_at")
            .HasColumnType("timestamptz");

        builder
            .Property(c => c.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamptz");

        builder.HasQueryFilter(i => !i.IsUsed && i.ExpiresAt > DateTime.UtcNow);
        builder.HasQueryFilter(i => !i.DeletedAt.HasValue);
    }
}