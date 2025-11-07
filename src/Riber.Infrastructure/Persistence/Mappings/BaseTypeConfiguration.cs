using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Abstractions;

namespace Riber.Infrastructure.Persistence.Mappings;

public abstract class BaseTypeConfiguration<T> : IEntityTypeConfiguration<T>
    where T : Tracker
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(GetTableName());

        builder.HasKey(x => x.Id)
            .HasName($"pk_{GetTableName()}_id");

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("modified_at")
            .HasColumnType("timestamptz");

        builder.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamptz");

        Mapping(builder);
        ConfigureQueryFilter(builder);
    }

    /// <summary>
    /// Obtém o nome da tabela no banco de dados para o tipo mapeado.
    /// </summary>
    /// <returns>
    /// O nome da tabela correspondente.
    /// </returns>
    protected abstract string GetTableName();

    /// <summary>
    /// Aplica configurações específicas de mapeamento para o tipo.
    /// Sobrescreva este método para definir mapeamentos de propriedades, relações e outras configurações personalizadas.
    /// </summary>
    /// <param name="builder">
    /// O construtor de configuração do tipo de entidade.
    /// </param>
    protected abstract void Mapping(EntityTypeBuilder<T> builder);

    /// <summary>
    /// Configura filtros de consulta globais para o tipo.
    /// Por padrão, aplica um filtro para excluir registros marcados como deletados.
    /// Sobrescreva este método para implementar filtros adicionais ou personalizados.
    /// </summary>
    /// <param name="builder">
    /// O construtor de configuração do tipo de entidade.
    /// </param>
    protected virtual void ConfigureQueryFilter(EntityTypeBuilder<T> builder)
        => builder.HasQueryFilter(x => !x.DeletedAt.HasValue);
}