using System;
using System.Linq;
using System.Threading.Tasks;

using ContosoUniversityFull.Models;

namespace ContosoUniversityFull.Services
{
    public interface IPictureDataService
    {
        Task<Picture> GetPictureAsync(int id);

        void DeletePictureCache(int id);
    }
}