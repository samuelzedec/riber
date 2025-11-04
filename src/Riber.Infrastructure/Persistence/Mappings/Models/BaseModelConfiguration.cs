using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Infrastructure.Persistence.Models;

namespace Riber.Infrastructure.Persistence.Mappings.Models;

public abstract class BaseModelConfiguration<T> : IEntityTypeConfiguration<T>
    where T : BaseModel
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

        ConfigureModel(builder);
        builder.HasQueryFilter(x => !x.DeletedAt.HasValue);
    }

    /// <summary>
    /// Recupera o nome da tabela do banco de dados associada a um tipo de model específico.
    /// Este método deve ser sobrescrito nas classes derivadas para especificar o nome da tabela
    /// que mapeia para a model.
    /// </summary>
    /// <returns>
    /// O nome da tabela correspondente à model.
    /// </returns>
    protected abstract string GetTableName();

    /// <summary>
    /// Configura as propriedades da entidade específica dentro da configuração do modelo.
    /// Este método deve ser implementado em classes derivadas para definir os mapeamentos
    /// adicionais e configurações específicas para a entidade na base de dados.
    /// </summary>
    /// <param name="builder">
    /// Objeto <see cref="EntityTypeBuilder&lt;T&gt;"/> usado para configurar a entidade do banco de dados.
    /// </param>
    protected abstract void ConfigureModel(EntityTypeBuilder<T> builder);
}