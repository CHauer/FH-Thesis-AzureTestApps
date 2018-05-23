using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversityCore.Models
{
    public class Picture
    {
        public int PictureID { get; set; }

        public string ContentType { get; set; }

        [Obsolete]
        public byte[] OriginalData { get; set; }

        [Obsolete]
        public byte[] Data { get; set; }

        [Obsolete]
        public byte[] ThumbnailData { get; set; }

        public string StoragePath { get; set; }

        public string OriginalUrl { get; set; }

        public string PictureUrl { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
