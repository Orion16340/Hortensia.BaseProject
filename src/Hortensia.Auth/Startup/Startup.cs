using Hortensia.Auth.Network;
using Hortensia.Core;
using Hortensia.ORM;
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
                .AddORMInfrastructure()
                .AddSingleton<AuthServer>()
                .AddSingleton<LifeTime>();

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
