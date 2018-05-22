using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ContosoUniversityCore.Models;

namespace ContosoUniversityCore.Services
{
    public interface IStudentDataService
    {
        Task<Student> GetStudentAsync(int id);

        Task UpdateStudentAsync(Student entity, Picture userPicture);

        Task<List<Course>> GetStudentSuggestedCoursesAsync(int id);

        Task<List<Department>> GetStudentDepartmentsAsync(int id);

        void DeleteStudentCache(int id);

        void DeleteStudentSuggestedCoursesCache(int id);

        void DeleteStudentDepartsmentsCache(int id);
    }
}