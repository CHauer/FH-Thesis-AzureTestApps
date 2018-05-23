using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoUniversityCore.Data;
using Microsoft.Azure.WebJobs;

namespace GenerateImageWebJobCore
{
    public class Functions
    {
        private readonly SchoolContext context;

        public Functions(SchoolContext context)
        {
            this.context = context;
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public void ProcessQueueMessage([QueueTrigger("queueappa")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }
    }
}
