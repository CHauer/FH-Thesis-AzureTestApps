using System.Threading.Tasks;
using System.Web;
using ContosoUniversityFull.Models;

namespace ContosoUniversityFull.Services
{
    public interface IUserPictureService
    {
        Task<Picture> CreateUserPictureAsync(HttpPostedFileBase picture);
    }
}