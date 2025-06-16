using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        ICategoryRepository Categories { get; }
        ICartRepository Carts { get; }
        IActivityLogRepository ActivityLogs { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IContactMessageRepository ContactMessages { get; }
        IFaqRepository Faqs { get; }
        INewsletterSubscriptionRepository NewsletterSubscriptions { get; }
        IOrderStatusHistoryRepository OrderStatusHistorys { get; }
        ISliderRepository Sliders { get; }
        IPaymentRepository paymentRepository { get; }
        IShippingAddressRepository ShippingAddresses { get; }
        ICartItemRepository CartItems { get; }
        IOrderItemRepository OrderItems { get; }
        IBlogRepository Blogs { get; }
        IAboutInfoRepository AboutInfos { get; }
        ITestimonialRepository Testimons { get; }
        IAdminUserRepository AdminUsers { get; }
        IWishlistRepository Wishlist { get; }

        Task<int> CompleteAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
