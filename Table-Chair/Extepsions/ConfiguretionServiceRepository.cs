using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Table_Chair.AutoMappers;
using Table_Chair_Application.Repositorys;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Application.PasswordHash;
using Table_Chair_Application.Emails;
using Table_Chair_Application.Settings.Table_Chair_Application.Settings;
using Table_Chair_Application.Settings;
using Table_Chair_Application.Tokens;
using Table_Chair_Application.Payments;
using Table_Chair_Application.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Table_Chair.Extepsions
{
    public static class ConfiguretionServiceRepository
    {
        public static IServiceCollection AddConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();
            services.AddTransient<DataSeeder>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.Scan(scan => scan
                .FromAssemblies(typeof(UserRepository).Assembly) // Repository joylashgan project
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
                .AsMatchingInterface()
                .WithLifetime(ServiceLifetime.Scoped));
            // Servicelarni qo‘shish
            services.Scan(scan => scan
                .FromAssemblies(typeof(UserService).Assembly) // Service joylashgan project
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Service")))
                .AsMatchingInterface()
                .WithLifetime(ServiceLifetime.Scoped));
            // AutoMapperni qo‘shish
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(typeof(MapperProfile));

            //Swaggerni qo‘shish
            services.AddSwaggerConfigurations();

            //DbContextni qo‘shish
            services.AddDbContext<FurnitureDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


            return services;
        }
    }
}
