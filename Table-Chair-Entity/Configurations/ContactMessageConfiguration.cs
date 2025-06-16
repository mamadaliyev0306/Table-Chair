using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Table_Chair_Entity.Models;

public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.HasKey(cm => cm.Id);
        builder.Property(cm => cm.FirstName).IsRequired().HasMaxLength(200);
        builder.Property(cm => cm.LastName).HasMaxLength(200);
        builder.Property(cm => cm.Email).IsRequired();
        builder.Property(cm => cm.Message).IsRequired();
        builder.Property(cm => cm.SentAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(cm => cm.IsDeleted).HasDefaultValue(false);
        builder.Property(cm => cm.IsRead).HasDefaultValue(false);
        builder.Property(cm => cm.IsResponded).HasDefaultValue(false);
        builder.Property(cm => cm.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(cm => cm.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

    }
}

