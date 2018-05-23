using ContosoUniversityCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GenerateImageWebJobCore.Services
{
    public interface IUserPictureService
    {
        Task GenerateUserPicture(int pictureId);
    }
}