using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(o => o.Status).HasDefaultValue(OrderStatus.Created);
        builder.Property(o => o.PaymentMethod).HasDefaultValue(PaymentMethod.CreditCard);
        builder.Property(o => o.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(o => o.IsDeleted).HasDefaultValue(false);
        builder.Property(o => o.CancelledAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.ShippingAddress)
            .WithMany(sa => sa.Orders)
            .HasForeignKey(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

