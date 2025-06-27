using Serilog;
using Serilog.Events;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Table_Chair.Extensions
{
    public static class AppSerilogConfiguration
    {
        public static void SeriloConfig(this IHostBuilder builder,IConfiguration config)
        {
            builder.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(config)
                    .Enrich.FromLogContext();
            });
        }
    }
}

