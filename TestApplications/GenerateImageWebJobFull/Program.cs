using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenerateImageWebJobFull.Infrastructure;
using Microsoft.Azure.WebJobs;

namespace GenerateImageWebJobFull
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration
            {
                JobActivator = new UnityActivator(UnityConfig.GetConfiguredContainer())
            };
            config.Queues.MaxPollingInterval = TimeSpan.FromSeconds(3);
            config.Queues.BatchSize = 30;

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
