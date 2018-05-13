using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversityCore.Models.SchoolViewModels
{
    public class CourseDetailsViewModel
    {
        public Course Course { get; set; }

        public List<Course> OtherCourses { get; set; }
    }
}
