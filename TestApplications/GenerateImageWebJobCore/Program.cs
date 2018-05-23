using System;
using System.IO;
using ContosoUniversityCore.Data;
using GenerateImageWebJobCore.Infrastructure;
using GenerateImageWebJobCore.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenerateImageWebJobCore
{
    class Program
    {
        private static IConfiguration configuration;

        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var jobconfiguration = new JobHostConfiguration();
            jobconfiguration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(3);
            jobconfiguration.Queues.BatchSize = 30;
            jobconfiguration.Queues.MaxDequeueCount = 5;
            jobconfiguration.JobActivator = new CustomJobActivator(serviceCollection.BuildServiceProvider());

            jobconfiguration.StorageConnectionString = configuration.GetConnectionString("AzureStorage");
            jobconfiguration.DashboardConnectionString = configuration.GetConnectionString("AzureStorage");
            jobconfiguration.UseCore();

            //configuration.UseTimers();

            var host = new JobHost(jobconfiguration);
            Console.WriteLine("WebJobs Host starts...");
            host.RunAndBlock();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Setup your container here, just like a asp.net core app
            // Optional: Setup your configuration:
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            serviceCollection.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            //serviceCollection.Configure<MySettings>(configuration);

            // Your classes that contain the webjob methods need to be DI-ed up too
            serviceCollection.AddTransient<Functions>();
            serviceCollection.AddTransient<IUserPictureService, UserPictureService>();

            // One more thing - tell azure where your azure connection strings are
            // redirected from azurestroage in appsettings to needed config variabeld jobsdash & jobs storage
            //Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configuration.GetConnectionString("AzureStorage"));
            //Environment.SetEnvironmentVariable("AzureWebJobsStorage", configuration.GetConnectionString("AzureStorage"));
        }
    }
}
