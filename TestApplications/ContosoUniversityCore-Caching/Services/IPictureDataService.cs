using System;
using System.Linq;
using System.Threading.Tasks;

using ContosoUniversityCore.Models;

namespace ContosoUniversityCore.Services
{
    public interface IPictureDataService
    {
        Task<Picture> GetPictureAsync(int id);

        void DeletePictureCache(int id);
    }
}