using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities;
using Riber.Infrastructure.Persistence.Extensions;

namespace Riber.Infrastructure.Persistence.Mappings;

public sealed class ImageMap : BaseEntityConfiguration<Image>
{
    protected override string GetTableName()
        => "image";

    protected override void ConfigureEntity(EntityTypeBuilder<Image> builder)
    {
        builder.ConfigureContentType();

        builder
            .Property(i => i.ShouldDelete)
            .HasColumnName("should_delete")
            .HasColumnType("boolean")
            .IsRequired();

        builder
            .Property(i => i.Length)
            .HasColumnName("length")
            .HasColumnType("bigint")
            .IsRequired();

        builder
            .Property(i => i.OriginalName)
            .HasColumnName("original_name")
            .HasColumnType("text")
            .IsRequired();

        builder
            .Property(i => i.Extension)
            .HasColumnName("extension")
            .HasColumnType("text")
            .IsRequired();

        builder
            .Property(i => i.MarkedForDeletionAt)
            .HasColumnName("marked_for_deletion_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);
    }
}