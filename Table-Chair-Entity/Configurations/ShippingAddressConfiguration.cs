using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
{
    public void Configure(EntityTypeBuilder<ShippingAddress> builder)
    {
        builder.HasKey(sa => sa.Id);
        builder.Property(sa => sa.RecipientName).IsRequired();
        builder.Property(sa => sa.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(sa => sa.AddressLine).IsRequired();
        builder.Property(sa => sa.City).IsRequired();
        builder.Property(sa => sa.Region).IsRequired();
        builder.Property(sa => sa.PostalCode).IsRequired();
        builder.Property(sa => sa.Country).HasDefaultValue(CountryMethod.Uzbekistan);
        builder.Property(sa => sa.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(sa => sa.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
