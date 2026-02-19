using Application.Interfaces.Contracts.Service;
using Infrastructure.Localization;
using Persistance.Repositories;
using System.Reflection;

namespace InventoryManagement.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddProjectModules(this IServiceCollection services)
        {
            var applicationAssembly = Assembly.Load("Application");
            // We scan the projects that contain the ACTUAL CLASSES (.cs files)
            var assemblies = new[]
            {
            Assembly.GetAssembly(typeof(UnitOfWork)),           // Persistence
            Assembly.GetAssembly(typeof(AppLocalizer)),         // Infrastructure
            applicationAssembly // Application
        };

            foreach (var assembly in assemblies)
            {
                if (assembly == null) continue;

                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract &&
                               (t.Name.EndsWith("Repository") || t.Name.EndsWith("Service")));

                foreach (var implementationType in types)
                {
                    // Look for the matching interface (e.g., IInvUserRepository)
                    // This interface is located in your Application.Interfaces layer
                    var interfaceType = implementationType.GetInterfaces()
                        .FirstOrDefault(i => i.Name == "I" + implementationType.Name);

                    if (interfaceType != null)
                    {
                        services.AddScoped(interfaceType, implementationType);
                    }
                }
            }
        }
    }
}
