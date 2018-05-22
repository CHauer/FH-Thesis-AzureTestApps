using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversityFull.Models.SchoolViewModels
{
    public class StudentDashboardViewModel
    {
        public Student Student { get; set; }

        public List<Department> StudentDepartments { get; set; }

        public List<Course> SuggestedCourses { get; set; }
    }
}
