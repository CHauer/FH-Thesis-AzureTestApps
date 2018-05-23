using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using ContosoUniversityFull.DAL;
using ContosoUniversityFull.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace ContosoUniversityFull.Services
{
    public class UserPictureService : IUserPictureService
    {
        private readonly SchoolContext context;

        private readonly IQueueSendClient<PictureJob> queueClient;

        public UserPictureService(SchoolContext context, IQueueSendClient<PictureJob> queueClient)
        {
            this.context = context;
            this.queueClient = queueClient;
        }

        public async Task EnqueuePictureJobAsync(int pictureId)
        {
            var load = new PictureJob()
            {
                PictureId = pictureId,
            };

            await queueClient.EnqueueMessageAsync(load);
        }

        public async Task GenerateUserPicture(int pictureId)
        {
            var userPicture = await context.Pictures.SingleOrDefaultAsync(p => p.PictureID == pictureId);

            if (userPicture == null)
            {
                return;
            }

            userPicture.Data = GeneratePicture(userPicture.OriginalData, 250, 350);
            userPicture.ThumbnailData = GeneratePicture(userPicture.OriginalData, 50, 50);

            await context.SaveChangesAsync();
        }

        private byte[] GeneratePicture(byte[] inputData, int width, int height)
        {
            using (Image<Rgba32> image = Image.Load(inputData))
            {
                image.Mutate(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Crop, Size = new Size(width, height) }));

                using (var memoryStream = new MemoryStream())
                {
                    image.SaveAsJpeg(memoryStream);
                    return memoryStream.ToArray();
                }
            }

        }
    }
}