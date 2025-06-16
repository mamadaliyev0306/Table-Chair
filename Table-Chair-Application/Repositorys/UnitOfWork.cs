using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FurnitureDbContext _context;

        public UnitOfWork(FurnitureDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Products = new ProductRepository(_context);
            Orders = new OrderRepository(_context);
            Categories = new CategoryRepository(_context);
            Carts = new CartRepository(_context);
            ActivityLogs=new ActivityLogRepository(_context);
            RefreshTokens = new RefreshTokenRepository(_context);
            ContactMessages = new ContactMessageRepository(_context);
            Faqs= new FaqRepository(_context);
            NewsletterSubscriptions = new NewsletterSubscriptionRepository(_context);
            OrderStatusHistorys = new OrderStatusHistoryRepository(_context);
            Sliders= new SliderRepository(_context);
            paymentRepository = new PaymentRepository(_context);
            ShippingAddresses= new ShippingAddressRepository(_context);
            OrderItems = new OrderItemRepository(_context);
            CartItems = new CartItemRepository(_context);
            Blogs= new BlogRepository(_context);
            Testimons = new TestimonialRepository(_context);
            AboutInfos = new AboutInfoRepository(_context);
            AdminUsers = new AdminUserRepository(_context);
            Wishlist = new WishlistRepository(_context);    
        }
        public IPaymentRepository paymentRepository { get; }
        public IUserRepository Users { get; }
        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public ICategoryRepository Categories { get; }
        public ICartRepository Carts { get; }

        public IActivityLogRepository ActivityLogs {  get; }
        public IRefreshTokenRepository RefreshTokens { get; }

        public IContactMessageRepository ContactMessages { get; }
        public IFaqRepository Faqs { get; }
        public INewsletterSubscriptionRepository NewsletterSubscriptions { get; }
        public IOrderStatusHistoryRepository OrderStatusHistorys { get; }
        public ISliderRepository Sliders { get; }
        public IShippingAddressRepository ShippingAddresses { get; }
        public IOrderItemRepository OrderItems { get; }
        public ICartItemRepository CartItems { get; }
        public IBlogRepository Blogs { get; }
        public ITestimonialRepository Testimons { get; }
        public IAboutInfoRepository AboutInfos { get; }
        public IAdminUserRepository AdminUsers { get; }
        public IWishlistRepository Wishlist { get; }
        public async Task<int> CompleteAsync() =>
            await _context.SaveChangesAsync();

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
        // Transaction boshqaruvi

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
