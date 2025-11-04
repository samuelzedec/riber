using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Infrastructure.Extensions;
using Riber.Infrastructure.Persistence.Models.Embeddings;

namespace Riber.Infrastructure.Persistence.Mappings.Models;

public sealed class ProductEmbeddingsModelMap
    : BaseModelConfiguration<ProductEmbeddingsModel>
{
    protected override string GetTableName()
        => "product_embeddings";

    protected override void ConfigureModel(EntityTypeBuilder<ProductEmbeddingsModel> builder)
    {
        builder.ConfigureVector();

        builder
            .Property(p => p.ProductId)
            .HasColumnName("product_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .HasIndex(x => x.ProductId)
            .HasDatabaseName("ix_product_embeddings_product_id");

        builder
            .HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.ProductId)
            .HasConstraintName("fk_product_embeddings_product_id")
            .OnDelete(DeleteBehavior.NoAction);
    }
}