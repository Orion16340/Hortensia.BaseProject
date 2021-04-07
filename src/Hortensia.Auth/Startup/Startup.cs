using Hortensia.Auth.ExempleDI;
using Hortensia.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Hortensia.Auth
{
    internal class Startup
    {
        public static Action<IServiceCollection> ConfigureServices { get; } = (services) =>
        {
            services
                .AddCoreInfrastructure()
                .AddSingleton<Exemple>();

            ServiceLocator.Provider = services.BuildServiceProvider();
        };

        public static Action<IConfigurationBuilder> ConfigureSettings { get; } = (settings) =>
        {
            var workingDirectory = Directory.GetCurrentDirectory();

            settings
                .AddJsonFile($"{workingDirectory}/Startup/appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        };
    }
}
