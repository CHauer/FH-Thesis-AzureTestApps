using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoUniversityCore.Data;
using ContosoUniversityCore.Services;
using GenerateImageWebJobCore.Services;
using Microsoft.Azure.WebJobs;

namespace GenerateImageWebJobCore
{
    public class Functions
    {
        private readonly IUserPictureService userPictureService;

        public Functions(IUserPictureService userPictureService)
        {
            this.userPictureService = userPictureService;
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public async Task ProcessQueueMessage([QueueTrigger("queueappb")] PictureJob job, TextWriter log)
        {
            try
            {
                await userPictureService.GenerateUserPicture(job.PictureId);
                log.WriteLine($"Image proccesed ID:{job.PictureId}");
            }
            catch (Exception ex)
            {
                log.WriteLine($"Error: {ex.Message}");
                throw ex;
            }
        }
    }
}
