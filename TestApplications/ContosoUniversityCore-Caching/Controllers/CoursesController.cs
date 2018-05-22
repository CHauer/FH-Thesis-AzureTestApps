using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using ContosoUniversityCore.Data;
using ContosoUniversityCore.Models;
using ContosoUniversityCore.Models.SchoolViewModels;
using ContosoUniversityCore.Services;

namespace ContosoUniversityCore.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext context;

        private readonly ICourseDataService courseService;

        private readonly IDepartmentDataService departmentService;

        private readonly IStudentDataService studentService;

        public CoursesController(SchoolContext context, ICourseDataService courseService, IDepartmentDataService departmentService, IStudentDataService studentService)
        {
            this.context = context;
            this.courseService = courseService;
            this.departmentService = departmentService;
            this.studentService = studentService;
        }

        // GET: Courses
        public async Task<ActionResult> Index(int? SelectedDepartment, int? studentId)
        {
            ViewBag.StudentId = studentId;

            var departments = await departmentService.GetDepartmentsAsync();
            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);

            int departmentID = SelectedDepartment.GetValueOrDefault();

            IQueryable<Course> courses = context.Courses
                .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentID)
                .OrderBy(d => d.CourseID)
                .Include(d => d.Department);
            var sql = courses.ToString();
            return View(courses.ToList());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id, int? studentId)
        {
            ViewBag.StudentId = studentId;

            if (id == null)
            {
                return NotFound();
            }

            CourseDetailsViewModel model = new CourseDetailsViewModel();

            model.Course = await courseService.GetCourseAsync(id.GetValueOrDefault());

            if (model.Course == null)
            {
                return NotFound();
            }

            model.OtherCourses = await courseService.GetCourseRecommendationsAsync(model.Course.CourseID, model.Course.DepartmentID);

            return View(model);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Credits,DepartmentID,Title")] Course course)
        {
            if (ModelState.IsValid)
            {
                context.Add(course);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await context.Courses
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseToUpdate = await context.Courses
                .SingleOrDefaultAsync(c => c.CourseID == id);

            if (await TryUpdateModelAsync<Course>(courseToUpdate,
                "",
                c => c.Credits, c => c.DepartmentID, c => c.Title))
            {
                try
                {
                    await context.SaveChangesAsync();
                    courseService.DeleteCourseCache(id.GetValueOrDefault());
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        private async void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departments = await departmentService.GetDepartmentsAsync();

            ViewBag.DepartmentID = new SelectList(departments, "DepartmentID", "Name", selectedDepartment);
        }


        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await context.Courses.SingleOrDefaultAsync(m => m.CourseID == id);
            context.Courses.Remove(course);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
            {
                ViewData["RowsAffected"] =
                    await context.Database.ExecuteSqlCommandAsync(
                        "UPDATE Course SET Credits = Credits * {0}",
                        parameters: multiplier);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId, int studentId)
        {
            if (ModelState.IsValid)
            {
                if (!context.Enrollments.Any(e => e.CourseID == courseId && e.StudentID == studentId))
                {
                    var newEnrollment = new Enrollment() { CourseID = courseId, StudentID = studentId };

                    context.Enrollments.Add(newEnrollment);
                    await context.SaveChangesAsync();

                    studentService.DeleteStudentSuggestedCoursesCache(studentId);

                    TempData["Success"] = $"Student successfully enrolled for Couse.";
                }
            }
            return RedirectToAction(nameof(Details), new { id = courseId, studentId });
        }

    }
}
