using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities;

namespace Riber.Infrastructure.Persistence.Mappings.Entities;

/// <summary>
/// Fornece uma implementação base de configuração para entidades que herdam de BaseEntity.
/// Esta classe é usada para definir mapeamentos de tabela comuns, configurações de chave primária
/// e outras configurações compartilhadas no model builder do Entity Framework Core.
/// Configurações específicas para uma entidade devem ser implementadas na classe derivada.
/// </summary>
/// <typeparam name="T">
/// O tipo da entidade para a qual esta configuração é definida.
/// Deve herdar de BaseEntity.
/// </typeparam>
public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : BaseEntity
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
        
        ConfigureEntity(builder);
        ConfigureQueryFilter(builder);
    }

    /// <summary>
    /// Recupera o nome da tabela do banco de dados associada a um tipo de entidade específico.
    /// Este método deve ser sobrescrito nas classes derivadas para especificar o nome da tabela
    /// que mapeia para a entidade.
    /// </summary>
    /// <returns>
    /// O nome da tabela correspondente à entidade.
    /// </returns>
    protected abstract string GetTableName();

    /// <summary>
    /// Aplica configurações adicionais para a entidade, como mapeamentos de propriedades,
    /// relações e outras definições específicas.
    /// Este método deve ser implementado em classes derivadas para fornecer configurações
    /// personalizadas para uma entidade específica.
    /// </summary>
    /// <param name="builder">
    /// O construtor de tipo de entidade utilizado para configurar a entidade no model builder
    /// do Entity Framework Core.
    /// </param>
    protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);

    /// <summary>
    /// Configura um filtro de consulta para a entidade durante sua configuração no Entity Framework Core.
    /// Permite a definição de condições específicas que serão aplicadas às consultas realizadas para esta entidade.
    /// Por padrão, aplica um filtro para ignorar entidades marcadas como excluídas.
    /// Pode ser sobrescrito em classes derivadas para implementar filtros específicos para outras condições.
    /// </summary>
    /// <param name="builder">
    /// O objeto <see cref="EntityTypeBuilder&lt;T&gt;"/> que fornece a API para configurar a entidade.
    /// </param>
    protected virtual void ConfigureQueryFilter(EntityTypeBuilder<T> builder)
        => builder.HasQueryFilter(x => !x.DeletedAt.HasValue);
}