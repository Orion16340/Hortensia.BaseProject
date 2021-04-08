using Microsoft.Extensions.DependencyInjection;

namespace Hortensia.Framing
{
    public static class FramingExtensions
    {
        public static IServiceCollection AddFramingInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IFrameManager, FrameManager>();
            
            return services;
        }
    }
}
