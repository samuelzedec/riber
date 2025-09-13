using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationUser = Riber.Infrastructure.Persistence.Identity.ApplicationUser;
using Identity = Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Persistence.Mappings.Identity;

public sealed class ApplicationUserMap : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("aspnet_user");

        builder
            .HasKey(u => u.Id)
            .HasName("pk_aspnet_user_id");

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
            .HasIndex(u => u.UserName, "uq_aspnet_user_user_name")
            .IsUnique();

        builder
            .Property(u => u.NormalizedUserName)
            .HasColumnType("text")
            .HasColumnName("normalized_user_name")
            .IsRequired();

        builder
            .HasIndex(u => u.NormalizedUserName, "uq_aspnet_user_normalized_user_name")
            .IsUnique();

        builder
            .Property(u => u.Email)
            .HasColumnType("text")
            .HasColumnName("email")
            .IsRequired();

        builder
            .HasIndex(u => u.Email, "uq_asp_net_user_email")
            .IsUnique();

        builder
            .Property(u => u.NormalizedEmail)
            .HasColumnType("text")
            .HasColumnName("normalized_email");
        
        builder
            .HasIndex(u => u.NormalizedEmail, "uq_asp_net_user_normalized_email")
            .IsUnique();

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
            .HasIndex(u => u.PhoneNumber, "uq_aspnet_user_phone_number")
            .IsUnique();

        builder
            .Property(u => u.PhoneNumberConfirmed)
            .HasColumnType("boolean")
            .HasColumnName("phone_number_confirmed")
            .HasDefaultValue(false);

        builder
            .Property(u => u.ConcurrencyStamp)
            .HasColumnType("varchar(36)")
            .HasColumnName("concurrency_stamp")
            .IsConcurrencyToken()
            .IsRequired();

        builder
            .Property(u => u.SecurityStamp)
            .HasColumnType("varchar(36)")
            .HasColumnName("security_stamp")
            .IsRequired();

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

        builder
            .Property(u => u.UserDomainId)
            .HasColumnType("uuid")
            .HasColumnName("user_domain_id")
            .IsRequired();
        
        builder
            .HasIndex(u => u.UserDomainId, "uq_aspnet_user_user_domain_id")
            .IsUnique();
        
        builder
            .HasOne(u => u.UserDomain)
            .WithOne()
            .HasForeignKey<ApplicationUser>(u => u.UserDomainId)
            .HasConstraintName("fk_aspnet_user_user_domain_id");
        
        builder
            .Property(u => u.IsDeleted)
            .HasColumnType("boolean")
            .HasColumnName("is_deleted")
            .IsRequired();
        
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}