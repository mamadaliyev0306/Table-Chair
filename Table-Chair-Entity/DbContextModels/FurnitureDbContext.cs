using Microsoft.EntityFrameworkCore;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

namespace Table_Chair_Entity.DbContextModels;

public class FurnitureDbContext : DbContext
{
    public FurnitureDbContext(DbContextOptions<FurnitureDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }
    public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }
    public DbSet<ShippingAddress> ShippingAddresses { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<AboutInfo> AboutInfos { get; set; }
    public DbSet<Faq> Faqs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<EmailVerification> EmailVerifications { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
                  .Property(u => u.Role)
                  .HasDefaultValue(Role.Customer)
                   .HasConversion<string>();
        // Asosiy entitylar uchun global filtrlar
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted && c.IsActive);
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted && u.IsActive);
        modelBuilder.Entity<Cart>().HasQueryFilter(c => c.User != null && !c.User.IsDeleted);

        // Bog'liq entitylar uchun filtrlar
        modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Category != null && !b.Category.IsDeleted && b.Category.IsActive);
        modelBuilder.Entity<Faq>().HasQueryFilter(f => f.Category != null && !f.Category.IsDeleted && f.Category.IsActive);

        // Cart va unga bog'liq filtrlar
        modelBuilder.Entity<CartItem>().HasQueryFilter(ci =>
            !ci.IsDeleted &&
            ci.Product != null && !ci.Product.IsDeleted &&
            ci.Cart != null && ci.Cart.User != null && !ci.Cart.User.IsDeleted);

        // Order va unga bog'liq filtrlar
        modelBuilder.Entity<Order>().HasQueryFilter(o =>
            !o.IsDeleted &&
            o.User != null && !o.User.IsDeleted);

        modelBuilder.Entity<OrderItem>().HasQueryFilter(oi =>
            oi.Product != null && !oi.Product.IsDeleted &&
            oi.Order != null && !oi.Order.IsDeleted &&
            oi.Order.User != null && !oi.Order.User.IsDeleted);

        modelBuilder.Entity<OrderStatusHistory>().HasQueryFilter(osh =>
            osh.Order != null && !osh.Order.IsDeleted &&
            osh.Order.User != null && !osh.Order.User.IsDeleted);

        // Payment uchun filter
        modelBuilder.Entity<Payment>().HasQueryFilter(p =>
            p.Order != null && !p.Order.IsDeleted &&
            p.Order.User != null && !p.Order.User.IsDeleted);

        // Wishlist uchun filtrlar
        modelBuilder.Entity<WishlistItem>().HasQueryFilter(wi =>
            wi.Product != null && !wi.Product.IsDeleted &&
            wi.User != null && !wi.User.IsDeleted);



        // RefreshToken uchun filter
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(rt =>
            !rt.IsRevoked &&
            rt.ExpiresAt > DateTime.UtcNow);


        // Qo'shimcha konfiguratsiyalar
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FurnitureDbContext).Assembly);
    }
}


