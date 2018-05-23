using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContosoUniversityCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace ContosoUniversityCore.Services
{

    public class UserPictureService : IUserPictureService
    {
        private readonly IConfiguration config;

        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        /// <value>
        /// The name of the container.
        /// </value>
        private string containerName = "picturesb";

        /// <summary>
        /// The BLOB container
        /// </summary>
        private CloudBlobContainer blobContainer;

        public UserPictureService(IConfiguration config)
        {
            this.config = config;
            InitializeStorage();
        }

        private async void InitializeStorage()
        {
            // Open storage account using credentials from .cscfg file.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.GetConnectionString("AzureStorage"));

            // Get context object for working with blobs, and 
            // set a default retry policy appropriate for a web user interface.
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to the blob container.
            blobContainer = blobClient.GetContainerReference(containerName);

            if (await blobContainer.CreateIfNotExistsAsync())
            {
                // configure containers blobs for public access
                var permissions = await blobContainer.GetPermissionsAsync();
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                await blobContainer.SetPermissionsAsync(permissions);
            }
        }

        public async Task<Picture> CreateUserPictureAsync(IFormFile picture)
        {
            if (picture == null || picture.Length <= 0)
            {
                return null;
            }

            var userPicture = new Picture()
            {
                ContentType = picture.ContentType
            };
            Guid pictureGuid = Guid.NewGuid();

            var pictureStream = picture.OpenReadStream();

            var origblob = await UploadBlobAsync(pictureStream, picture.FileName.Split('.').Last(), picture.ContentType, pictureGuid, "students/original");
            userPicture.OriginalUrl = origblob.Uri.ToString();
            userPicture.StoragePath = origblob.Name;

            pictureStream.Seek(0, SeekOrigin.Begin);
            var stream = GeneratePicture(pictureStream, 250, 350);
            stream.Seek(0, SeekOrigin.Begin);
            var blob = await UploadBlobAsync(stream, ".jpg", "image/jpeg", pictureGuid, "students/default");
            userPicture.PictureUrl = blob.Uri.ToString();

            pictureStream.Seek(0, SeekOrigin.Begin);
            var thumbnailstream = GeneratePicture(pictureStream, 50, 50);
            thumbnailstream.Seek(0, SeekOrigin.Begin);
            var thumbblob = await UploadBlobAsync(thumbnailstream, ".jpg", "image/jpeg", pictureGuid, "students/thumbnail");
            userPicture.ThumbnailUrl = thumbblob.Uri.ToString();

            return userPicture;
        }

        /// <summary>
        /// Uploads the BLOB asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        private async Task<CloudBlockBlob> UploadBlobAsync(Stream stream, string fileExtension, string contentType,
                                                                   Guid guid, string folder = null)
        {
            if (folder != null && folder.EndsWith(@"/"))
            {
                folder = folder.Remove(folder.Length - 1);
            }

            if (string.IsNullOrEmpty(fileExtension))
            {
                fileExtension = ".jpg";
            }

            if (!string.IsNullOrWhiteSpace(fileExtension) && !fileExtension.StartsWith("."))
            {
                fileExtension = "." + fileExtension;
            }

            // Creates {folder}/{Guid}{.ext} or  {guid}{.ext} if folder is null or empty or whitespace
            string blobName = $"{(!string.IsNullOrWhiteSpace(folder) ? folder + @"/" : string.Empty)}{guid}{fileExtension}";

            // Retrieve reference to a blob. 
            CloudBlockBlob uploadBlob = blobContainer.GetBlockBlobReference(blobName);
            uploadBlob.Properties.ContentType = contentType;

            await uploadBlob.UploadFromStreamAsync(stream);

            return uploadBlob;
        }

        private Stream GeneratePicture(Stream inputData, int width, int height)
        {
            using (Image<Rgba32> image = Image.Load(inputData))
            {
                image.Mutate(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Crop, Size = new Size(width, height) }));

                var memoryStream = new MemoryStream();

                image.SaveAsJpeg(memoryStream);
                return memoryStream;
            }

        }

    }
}