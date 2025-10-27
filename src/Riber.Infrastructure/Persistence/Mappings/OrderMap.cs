using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities;
using Riber.Infrastructure.Extensions;

namespace Riber.Infrastructure.Persistence.Mappings;

public sealed class OrderMap : BaseEntityConfiguration<Order>
{
    protected override string GetTableName()
        => "order";

    protected override void ConfigureEntity(EntityTypeBuilder<Order> builder)
    {
        builder.ConfigureRandomToken(GetTableName(), "order_token");
        
        builder
            .HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .HasConstraintName("fk_order_company_id")
            .IsRequired();
        
        builder
            .HasOne(x => x.Attendant)
            .WithMany()
            .HasForeignKey(x => x.AttendantId)
            .HasConstraintName("fk_order_attendant_id")
            .IsRequired();

        builder
            .HasMany<OrderItem>("_items")
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .HasConstraintName("fk_order_item_order_id")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .Metadata
            .FindNavigation("_items")!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        
        builder
            .Ignore(x => x.ItemsReadOnly)
            .Ignore(x => x.SubTotal)
            .Ignore(x => x.TotalDiscounts)
            .Ignore(x => x.TotalAmount);
    }
}