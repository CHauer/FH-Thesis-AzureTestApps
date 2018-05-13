using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoUniversityFull.Models
{
    public class Picture
    {
        public int PictureID { get; set; }

        public string ContentType { get; set; }

        public byte[] OriginalData { get; set; }

        public byte[] Data { get; set; }

        public byte[] ThumbnailData { get; set; }

        //public string PictureUrl { get; set; }
        //public string ThumbnailUrl { get; set; }
    }
}