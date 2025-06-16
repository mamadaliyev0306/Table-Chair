using Microsoft.OpenApi.Models;

namespace Table_Chair.Extepsions
{
    public static class DISwaggerConfiguretion
    {
        public static IServiceCollection AddSwaggerConfigurations(this IServiceCollection services)
        {
            // Swagger uchun JWT Authentication qo‘shish
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Table_Chair_API", Version = "v1" });

                // JWT Authentication konfiguratsiyasi
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: \"Bearer eyJhbGciOi...\"",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });
            return services;
        }
    }
}
