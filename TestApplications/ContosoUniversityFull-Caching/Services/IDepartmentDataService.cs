using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ContosoUniversityFull.Models;

namespace ContosoUniversityFull.Services
{
    public interface IDepartmentDataService
    {
        Task<List<Department>> GetDepartmentsAsync();

        void DeleteDepartmentsCache();
    }
}