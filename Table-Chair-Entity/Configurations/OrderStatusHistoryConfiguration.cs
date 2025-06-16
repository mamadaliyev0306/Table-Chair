using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.HasKey(osh => osh.Id);
        builder.Property(osh => osh.Status).HasDefaultValue(OrderStatus.Created);
        builder.Property(osh => osh.ChangedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(osh => osh.Order)
            .WithMany(o => o.StatusHistories)
            .HasForeignKey(osh => osh.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

