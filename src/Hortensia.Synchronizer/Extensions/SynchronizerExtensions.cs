using Hortensia.Synchronizer.Commands;
using Hortensia.Synchronizer.Parameters;
using Microsoft.Extensions.DependencyInjection;

namespace Hortensia.Synchronizer.Extensions
{
    public static class SynchronizerExtensions
    {
        public static IServiceCollection AddSynchronizerInfrastructure(this IServiceCollection services)
        {
            var options = new NetworkOptions
            {
                IP = "127.0.0.1",
                Port = 446,
                Backlog = 100,
                BufferLength = 4096,
                MaxConcurrentConnections = 100,
                MaxConnectionsPairIP = 8
            };

            services.AddSingleton<INetworkOptions>(options);
            services.AddSingleton<IConsoleCommandsManager, ConsoleCommandsManager>();

            return services;
        }
    }
}
