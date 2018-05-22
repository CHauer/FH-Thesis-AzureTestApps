using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ContosoUniversityCore.Models;

namespace ContosoUniversityCore.Services
{
    public interface IDepartmentDataService
    {
        Task<List<Department>> GetDepartmentsAsync();

        void DeleteDepartmentsCache();
    }
}