using System;
using System.IO;
using GenerateImageWebJobCore.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenerateImageWebJobCore
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var configuration = new JobHostConfiguration();
            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(3);
            configuration.Queues.BatchSize = 1;
            configuration.JobActivator = new CustomJobActivator(serviceCollection.BuildServiceProvider());
            //configuration.UseTimers();

            var host = new JobHost(configuration);
            host.RunAndBlock();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Setup your container here, just like a asp.net core app

            // Optional: Setup your configuration:
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            //serviceCollection.Configure<MySettings>(configuration);

            // A silly example of wiring up some class used by the web job:
            //serviceCollection.AddScoped<ISomeInterface, SomeUsefulClass>();

            // Your classes that contain the webjob methods need to be DI-ed up too
            serviceCollection.AddScoped<Functions, Functions>();

            // One more thing - tell azure where your azure connection strings are
            Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configuration.GetConnectionString("WebJobsDashboard"));
            Environment.SetEnvironmentVariable("AzureWebJobsStorage", configuration.GetConnectionString("WebJobsStorage"));
        }
    }
}
