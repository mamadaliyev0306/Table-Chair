using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Table_Chair_Entity.Models;

public class FaqConfiguration : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Title).IsRequired().HasMaxLength(300);
        builder.Property(f => f.Answer).IsRequired();
        builder.Property(f => f.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(f => f.IsDeleted).HasDefaultValue(false);

        builder.HasOne(f => f.Category)
            .WithMany(c => c.Faqs)
            .HasForeignKey(f => f.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

