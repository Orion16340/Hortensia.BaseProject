using Hortensia.Synchronizer.Parameters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hortensia.Synchronizer.Extensions
{
    public static class SynchronizerExtensions
    {
        public static IServiceCollection ConfigureNetwork(this IServiceCollection services)
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

            services.AddSingleton(options);

            return services;
        }
    }
}
