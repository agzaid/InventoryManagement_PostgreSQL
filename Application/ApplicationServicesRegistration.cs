using Application.Interfaces.Contracts.Service;
using Application.Mappers;
using Application.Service;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


            //services.AddScoped<IInvUserService, InvUserService>();
            //services.AddScoped<IDepartmentService, DepartmentService>();
            //services.AddScoped<ISystemManagementService, SystemManagementService>();
            //services.AddScoped<IStoreService, StoreService>();
            return services;
        }
    }
}
