using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ContosoUniversityFull.Models;

namespace ContosoUniversityFull.ViewModels
{
    public class CourseDetailsViewModel
    {
        public Course Course { get; set; }

        public List<Course> OtherCourses { get; set; }
    }
}
