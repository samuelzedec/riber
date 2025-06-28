using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Identity = SnackFlow.Infrastructure.Persistence.Identity;

namespace SnackFlow.Infrastructure.Persistence.Mappings.Identity;

public sealed class ApplicationUserMap : IEntityTypeConfiguration<Identity::ApplicationUser>
{
    public void Configure(EntityTypeBuilder<Identity::ApplicationUser> builder)
    {
        builder.ToTable("aspnet_user");

        builder
            .Property(u => u.Id)
            .HasColumnType("uuid")
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");

        builder
            .Property(u => u.Name)
            .HasColumnType("text")
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(u => u.UserName)
            .HasColumnType("text")
            .HasColumnName("user_name")
            .IsRequired();

        builder
            .Property(u => u.NormalizedUserName)
            .HasColumnType("text")
            .HasColumnName("normalized_user_name")
            .IsRequired();

        builder
            .HasIndex(u => u.NormalizedUserName)
            .HasDatabaseName("ix_aspnet_user_normalized_user_name")
            .IsUnique();

        builder
            .Property(u => u.Email)
            .HasColumnType("text")
            .HasColumnName("email")
            .IsRequired();

        builder
            .HasIndex(u => u.Email)
            .HasDatabaseName("ix_asp_net_user_email")
            .IsUnique();

        builder
            .Property(u => u.NormalizedEmail)
            .HasColumnType("text")
            .HasColumnName("normalized_email");

        builder
            .Property(u => u.EmailConfirmed)
            .HasColumnType("boolean")
            .HasColumnName("email_confirmed")
            .IsRequired();

        builder
            .Property(u => u.PasswordHash)
            .HasColumnType("text")
            .HasColumnName("password_hash")
            .IsRequired();

        builder
            .Property(u => u.PhoneNumber)
            .HasColumnType("varchar(15)")
            .HasColumnName("phone_number")
            .IsRequired(false);

        builder
            .Property(u => u.PhoneNumberConfirmed)
            .HasColumnType("boolean")
            .HasColumnName("phone_number_confirmed")
            .HasDefaultValue(false);

        builder
            .Property(u => u.ConcurrencyStamp)
            .HasColumnType("varchar(36)")
            .HasColumnName("concurrency_stamp")
            .IsConcurrencyToken();

        builder
            .Property(u => u.SecurityStamp)
            .HasColumnType("varchar(36)")
            .HasColumnName("security_stamp");

        builder
            .Property(u => u.LockoutEnd)
            .HasColumnType("timestamptz")
            .HasColumnName("lockout_end");

        builder
            .Property(u => u.LockoutEnabled)
            .HasColumnType("boolean")
            .HasColumnName("lockout_enabled")
            .IsRequired();

        builder
            .Property(u => u.AccessFailedCount)
            .HasColumnType("integer")
            .HasColumnName("access_failed_count")
            .IsRequired();

        builder
            .Property(u => u.TwoFactorEnabled)
            .HasColumnType("boolean")
            .HasColumnName("two_factor_enabled")
            .IsRequired();
    }
}