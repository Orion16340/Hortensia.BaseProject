using Hortensia.Auth.Network;
using Hortensia.Core;
using Hortensia.Framing;
using Hortensia.ORM;
using Hortensia.Synchronizer.Extensions;
using Hortensia.Synchronizer.Parameters;
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
                .AddFramingInfrastructure()
                .ConfigureNetwork()
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
