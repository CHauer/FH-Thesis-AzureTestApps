using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoUniversityFull.DAL;
using ContosoUniversityFull.Services;
using Microsoft.Azure.WebJobs;

namespace GenerateImageWebJobFull
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
        public async Task ProcessQueueMessage([QueueTrigger("queueappa")] PictureJob job, TextWriter log)
        {
            await userPictureService.GenerateUserPicture(job.PictureId);
        }
    }
}
