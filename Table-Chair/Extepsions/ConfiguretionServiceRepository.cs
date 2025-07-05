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
using Table_Chair.Infrastructure.Logging.InterfaceCorrelationId;
using Table_Chair.Infrastructure.Logging;
using StackExchange.Redis;
using Table_Chair_Application.CacheServices;


namespace Table_Chair.Extepsions
{
    public static class ConfiguretionServiceRepository
    {
        public static IServiceCollection AddConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string is missing");


            services.AddMemoryCache(); // MemoryCache  

            // Redis konfiguratsiyasi  
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configOptions = ConfigurationOptions.Parse(redisConnectionString, true);
                configOptions.Ssl = true;
                configOptions.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(configOptions);
            });

            // Redis va Memory uchun alohida service lar  
            services.AddSingleton<RedisCacheService>();
            services.AddSingleton<MemoryCacheService>();

            // FallbackCacheService  
            services.AddSingleton<ICacheService>(sp =>
            {
                var redisService = sp.GetRequiredService<RedisCacheService>();
                var memoryService = sp.GetRequiredService<MemoryCacheService>();
                return new FallbackCacheService(redisService, memoryService);
            });

            // Redis Distributed Cache  
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "AuthService_";
            });

            // JWT konfiguratsiyasi  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
         .AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidIssuer = configuration["JwtSettings:Issuer"], 
            ValidateAudience = true,
            ValidAudience = configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
             IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]!)
            ),
              ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(15)
            };
          });

           // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy
                        .WithOrigins("https://tezshop.onrender.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddHttpClient<ClickGateway>();
            services.AddHttpClient<PaymeGateway>();
            services.AddHttpClient<PayPalGateway>();
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
         //   services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(typeof(MapperProfile));

            // Swaggerni qo‘shish  
            services.AddSwaggerConfigurations();

            // DbContextni qo‘shish  
            services.AddDbContext<FurnitureDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Filters  
            services.AddScoped<GlobalExceptionFilter>();

            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });

            // CorrelationId  
            services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

            return services;
        }
    }
}
