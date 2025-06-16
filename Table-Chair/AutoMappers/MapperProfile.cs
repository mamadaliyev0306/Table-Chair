using AutoMapper;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.DetailsDtos;
using Table_Chair_Entity.Models;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Application.Dtos.AboutInfoDtos;
using Table_Chair_Application.Dtos.BlogDtos;
using Table_Chair_Application.Dtos.ShippingAddressDtos;

namespace Table_Chair.AutoMappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Order, OrderDetailsDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => $"{ src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserPhone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.OrderItems.Sum(i => i.Quantity * i.UnitPrice)));

            CreateMap<OrderItem, OrderItemDetailsDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));

            CreateMap<Payment, PaymentDetailsDto>();

            // AboutInfo mappings
            CreateMap<AboutInfo, AboutInfoDto>().ReverseMap();

            // Blog mappings
            CreateMap<Blog, BlogDto>().ReverseMap();
            CreateMap<BlogPostCreateDto, Blog>();

            // CartItem mappings
            CreateMap<CartItem, CartItemDto>().ReverseMap();
            CreateMap<CartItemCreateDto, CartItem>();
            CreateMap<AddToCartDto, CartItem>();

            // Category mappings
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryCreateDto, Category>();

            // ContactMessage mappings
            CreateMap<ContactMessage, ContactMessageDto>().ReverseMap();
            CreateMap<ContactMessageCreateDto, ContactMessage>();
            CreateMap<ContactFormDto, ContactMessage>();

            // Faq mappings
            CreateMap<Faq, FaqDto>().ReverseMap();
            CreateMap<FaqCreateDto, Faq>();

            // NewsletterSubscription mappings
            CreateMap<NewsletterSubscription, NewsletterSubscriptionDto>().ReverseMap();
            CreateMap<NewsletterSubscriptionCreateDto, NewsletterSubscription>();

            // Order mappings
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Order, OrderResponseDto>();
            CreateMap<Order, OrderSummaryDto>();
            CreateMap<CreateOrderDto, Order>();

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<OrderItemCreateDto, OrderItem>();

            // Payment mappings
            CreateMap<Payment, PaymentResponseDto>().ReverseMap();
            CreateMap<Payment, PaymentHistoryDto>();

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.UserPhone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<ProductCreateDto, Product>().ReverseMap();
            CreateMap<CreateProductDto, Product>().ReverseMap();

            CreateMap<Product, ProductReviewDto>();

            // ShippingAddress mappings
            CreateMap<ShippingAddress, ShippingAddressDto>().ReverseMap();
            CreateMap<ShippingAddressCreateDto, ShippingAddress>().ReverseMap();

            // Slider mappings
            CreateMap<Slider, SliderDto>().ReverseMap();

            //OrderStatusHistoryService
            CreateMap<OrderStatusHistory, OrderStatusHistoryDto>();
            CreateMap<CreateOrderStatusHistoryDto, OrderStatusHistory>();

            // Testimonial mappings
            CreateMap<Testimonial, TestimonialDto>().ReverseMap();
            CreateMap<TestimonialCreateDto, Testimonial>();



            // WishlistItem mappings
            CreateMap<WishlistItem, WishlistItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.ImageUrl));
            CreateMap<AddToWishlistDto, WishlistItem>();
            CreateMap<WishlistItemCreateDto, WishlistItem>();
            CreateMap<WishlistItem, WishlistToggleResultDto>()
                         .ForMember(dest => dest.IsInWishlist, opt => opt.MapFrom(src => true));
            // Other DTO mappings
            CreateMap<CheckoutDto, Order>();
            CreateMap<PagedResultDto<Order>, PagedResultDto<OrderDto>>();
            CreateMap<PagedResultDto<Product>, PagedResultDto<ProductDto>>();


            CreateMap<Cart, CartDto>()
    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
    

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.Product.StockQuantity >= src.Quantity))
                .ForMember(dest => dest.AvailabilityMessage, opt => opt.MapFrom(src =>
                    src.Product.StockQuantity >= src.Quantity
                        ? "Mavjud"
                        : $"Faqat {src.Product.StockQuantity} dona mavjud"));
            //User
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserResponseDto, User>();
            CreateMap<User, UserResponseDto>();
            CreateMap<UserUpdateDto, User>();

            //Email
            CreateMap<VerifyEmailDto, EmailVerification>();
        }
    }
}
