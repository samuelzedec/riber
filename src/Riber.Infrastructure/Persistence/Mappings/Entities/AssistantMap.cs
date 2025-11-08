using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities.Chat;

namespace Riber.Infrastructure.Persistence.Mappings.Entities;

public sealed class AssistantMap : BaseTypeConfiguration<Assistant>
{
    protected override string GetTableName()
        => "assistant";

    protected override void Mapping(EntityTypeBuilder<Assistant> builder)
    {
        builder
            .Property(a => a.Name)
            .HasColumnName("name")
            .HasColumnType("text")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(a => a.SystemPrompt)
            .HasColumnName("system_prompt")
            .HasColumnType("text")
            .IsRequired();

        builder
            .Property(a => a.Type)
            .HasColumnName("type")
            .HasColumnType("text")
            .HasConversion<string>()
            .IsRequired();
    }
}