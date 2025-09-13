using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities;
using Riber.Infrastructure.Persistence.Extensions;

namespace Riber.Infrastructure.Persistence.Mappings;

public sealed class OrderItemMap : BaseEntityConfiguration<OrderItem>
{
    protected override string GetTableName()
        => "order_item";

    protected override void ConfigureEntity(EntityTypeBuilder<OrderItem> builder)
    {
        builder
            .ConfigureUnitPrice()
            .ConfigureQuantity()
            .ConfigureDiscount();
        
        builder
            .Property(x => x.OrderId)
            .HasColumnName("order_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .HasOne(x => x.Product)
            .WithOne()
            .HasForeignKey<OrderItem>(x => x.ProductId)
            .HasConstraintName("fk_order_item_product_id")
            .IsRequired();

        builder
            .Property(x => x.ProductName)
            .HasColumnName("product_name")
            .HasColumnType("text")
            .HasMaxLength(255);

        builder
            .Ignore(x => x.SubTotal)
            .Ignore(x => x.DiscountAmount)
            .Ignore(x => x.TotalPrice);
    }
}