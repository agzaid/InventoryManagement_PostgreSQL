using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Json;

namespace Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services)
        {
            // Register infrastructure services, db contexts, repositories, etc.

            return services;
        }

        public static IHostBuilder ConfigureSerilog(this IHostBuilder builder)
        {
            return builder.UseSerilog((context, services, configuration) =>
            {
                var env = context.HostingEnvironment;

                configuration
                    // 1. تحديد مستوى التسجيل الأدنى ليكون Warning بدلاً من Information (الافتراضي)
                    .MinimumLevel.Warning()

                    // 2. اختيارياً: يمكنك السماح لبرامجك الخاصة بتسجيل Information مع منع Microsoft
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)

                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("Application", "InventoryManagement.Api")
                    .Enrich.WithEnvironmentUserName();

                configuration.WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

                configuration.WriteTo.File(
                    new JsonFormatter(),
                    path: "Logs/structured-log.json",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30);
            });
        }

    }
}
