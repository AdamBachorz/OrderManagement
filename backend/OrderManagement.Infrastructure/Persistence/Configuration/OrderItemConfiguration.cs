using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Configuration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems", t =>
            {
                // Database-level constraints - the same rules as in the domain
                t.HasCheckConstraint("CK_OrderItems_Quantity_Positive", "\"Quantity\" > 0");
                t.HasCheckConstraint("CK_OrderItems_UnitPrice_NonNegative", "\"UnitPrice\" >= 0");
            });

            builder.HasKey(i => i.Id);

            // IMPORTANT: without this EF treats new items with a set Guid as existing (Modified)
            // and SaveChanges ends with DbUpdateConcurrencyException when editing the order.
            builder.Property(i => i.Id).ValueGeneratedNever();

            builder.Property(i => i.ProductName)
                .IsRequired()
                .HasMaxLength(OrderItem.ProductNameMaxLength);

            builder.Property(i => i.Quantity).IsRequired();

            builder.Property(i => i.UnitPrice)
                .HasPrecision(18, OrderItem.PriceScale)
                .IsRequired();

            builder.Ignore(i => i.LineTotal);
        }
    }
}
