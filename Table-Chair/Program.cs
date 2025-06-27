using Microsoft.AspNetCore.Authentication.JwtBearer; // Agar JWT ishlatilsa
using Microsoft.IdentityModel.Tokens; // Agar JWT ishlatilsa
using Serilog;
using StackExchange.Redis;
using System.Text;
using Table_Chair.Extensions;
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

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddConfigurationServices(configuration);
            builder.Services.AddMemoryCache();
            builder.Host.SeriloConfig(configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapOpenApi();
            }

            if (!app.Environment.IsProduction())
            {
                app.UseHttpsRedirection(); 
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
