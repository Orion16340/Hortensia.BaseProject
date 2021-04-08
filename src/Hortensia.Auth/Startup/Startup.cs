using Hortensia.Auth.Managers;
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
                .AddSynchronizerInfrastructure()
                .AddSingleton<AuthServer>()
                .AddSingleton<LifeTime>()
                .AddSingleton<AccountManager>();

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
