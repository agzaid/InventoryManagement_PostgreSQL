using Application.Interface.Contract.Persistance;
using Application.Interfaces.Contracts.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistance.Repositories;
using Oracle.EntityFrameworkCore; // The primary namespace
using Oracle.EntityFrameworkCore.Infrastructure; // Often contains configuration

namespace Persistance
{
    public static class PersistanceServicesRegistration
    {
        public static IServiceCollection ConfigurePersistanceServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<InventoryManagementDbContext>(opitons => options.)
            // Read connection string from appsettings.json
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Register DbContext with Oracle
            //services.AddDbContext<InventoryManagementDbContext>(options =>
            //{
            //    options.UseOracle(
            //        connectionString,
            //        oracleOptions =>
            //        {
            //            // --- THE FINAL FIX: Use the string literal "11" ---
            //            oracleOptions.UseOracleSQLCompatibility("11");
            //        }
            //    )
            //    // Keep logging enabled for diagnostic confirmation
            //    .EnableSensitiveDataLogging()
            //    .LogTo(Console.WriteLine, LogLevel.Information);
            //});

            //register dbcontext with postegresql
            services.AddDbContext<InventoryManagementDbContext>(options =>
            {
                options.UseNpgsql(connectionString)
                       .EnableSensitiveDataLogging()
                       .LogTo(Console.WriteLine, LogLevel.Information);
            });

            services.AddHttpContextAccessor();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            //services.AddScoped<IInvUserRepository, InvUserRepository>();
            //services.AddScoped<IEgxEmployeeRepository, EgxEmployeeRepository>();
            //services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
