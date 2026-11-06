using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedNever(); // Guid generuje domena

            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(Order.OrderNumberMaxLength);
            builder.HasIndex(o => o.OrderNumber).IsUnique();

            builder.Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(Order.CustomerNameMaxLength);

            builder.Property(o => o.CreatedAt).IsRequired(); // DateTimeOffset -> timestamptz

            builder.Property(o => o.Status)
                .HasConversion<string>() // readability in DB
                .HasMaxLength(32)
                .IsRequired();

            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => o.CreatedAt);

            builder.Ignore(o => o.TotalAmount); // computed value, not stored in DB

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(i => i.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // removing orphanet items

            // EF operates on the _items field, not on the read-only wrapper from the Items property
            builder.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
