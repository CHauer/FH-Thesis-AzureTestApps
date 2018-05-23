using System.Threading.Tasks;
using System.Web;
using ContosoUniversityCore.Models;
using Microsoft.AspNetCore.Http;

namespace ContosoUniversityCore.Services
{
    public interface IUserPictureService
    {
        Task<Picture> CreateUserPictureAsync(IFormFile picture);
    }
}