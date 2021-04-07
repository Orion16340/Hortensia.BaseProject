using Microsoft.Extensions.DependencyInjection;

namespace Hortensia.Core
{
    public static class CoreExtensions
    {
        public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ILogger, Logger>();
            return services;
        }
    }
}
