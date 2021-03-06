using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Hortensia.Core;
using Microsoft.AspNetCore.Hosting;

namespace Hortensia.Auth
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build();

            ServiceLocator.Provider.GetService<LifeTime>().Start();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webHostBuilder =>
            {
                webHostBuilder
                    .ConfigureServices(Startup.ConfigureServices)
                    .ConfigureAppConfiguration(Startup.ConfigureSettings);
            });
    }
}
