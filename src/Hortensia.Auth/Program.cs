using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Hortensia.Core;
using Hortensia.Auth.ExempleDI;
using Microsoft.AspNetCore.Hosting;

namespace Hortensia.Auth
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Hortensia.Auth";

            CreateHostBuilder(args).Build();

            ServiceLocator.Provider.GetService<Exemple>().Start();

            while (true)
                Console.ReadLine();
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
