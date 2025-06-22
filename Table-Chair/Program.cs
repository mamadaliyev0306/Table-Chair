using Microsoft.AspNetCore.Authentication.JwtBearer; // Agar JWT ishlatilsa
using Microsoft.IdentityModel.Tokens; // Agar JWT ishlatilsa
using StackExchange.Redis;
using System.Text;
using Table_Chair.Extepsions;
using Table_Chair_Application.CacheServices;

namespace Table_Chair
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            var redisConnectionString = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string is missing");

            builder.Services.AddMemoryCache(); // MemoryCache

            // Redis konfiguratsiyasi
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configOptions = ConfigurationOptions.Parse(redisConnectionString, true);
                configOptions.Ssl = true;
                configOptions.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(configOptions);
            });

            // Redis va Memory uchun alohida service lar
            builder.Services.AddSingleton<RedisCacheService>();
            builder.Services.AddSingleton<MemoryCacheService>();

            // FallbackCacheService
            builder.Services.AddSingleton<ICacheService>(sp =>
            {
                var redisService = sp.GetRequiredService<RedisCacheService>();
                var memoryService = sp.GetRequiredService<MemoryCacheService>();
                return new FallbackCacheService(redisService, memoryService);
            });

            //  Redis Distributed Cache
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "AuthService_";
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddConfigurationServices(configuration);
            builder.Services.AddMemoryCache();

            //  JWT konfiguratsiyasi
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing"))
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });

            //  CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.WithOrigins("https://tezshop.onrender.com")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            app.MapGet("/healthz", () => Results.Ok("Healthy"));
            app.MapGet("/", () => "Table Chair is running");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapOpenApi();
            }

            if (!app.Environment.IsProduction())
            {
                app.UseHttpsRedirection(); // Render prod muhitida ishlamaydi, ehtiyot bo‘ling
            }

            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddlewaresDI();
            app.MapControllers();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.Run();
        }
    }
}
