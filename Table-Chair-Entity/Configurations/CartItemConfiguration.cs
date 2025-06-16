using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Table_Chair_Entity.Models;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.CartItemId);
        builder.Property(ci => ci.Quantity).HasDefaultValue(1);
        builder.Property(ci => ci.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(ci => ci.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(ci => ci.IsDeleted).HasDefaultValue(false);

        builder.HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

