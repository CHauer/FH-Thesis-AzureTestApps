using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContosoUniversityCore.Data;
using ContosoUniversityCore.Models;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace GenerateImageWebJobCore.Services
{
    public class UserPictureService : IUserPictureService
    {
        private readonly SchoolContext context;

        public UserPictureService(SchoolContext context)
        {
            this.context = context;
        }

        public async Task GenerateUserPicture(int pictureId)
        {
            var userPicture = await context.Pictures.SingleOrDefaultAsync(p => p.PictureID == pictureId).ConfigureAwait(false);

            if (userPicture == null)
            {
                return;
            }

            userPicture.Data = GeneratePicture(userPicture.OriginalData, 250, 350);
            userPicture.ThumbnailData = GeneratePicture(userPicture.OriginalData, 50, 50);

            await context.SaveChangesAsync().ConfigureAwait(false);
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