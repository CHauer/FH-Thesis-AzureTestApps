using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

using ContosoUniversityFull.DAL;
using ContosoUniversityFull.Models;
using ContosoUniversityFull.Services;
using ContosoUniversityFull.ViewModels;
using System.Threading.Tasks;

namespace ContosoUniversityFull.Controllers
{
    public class CoursesController : Controller
    {
        private SchoolContext db;

        private readonly ICourseDataService courseService;

        private readonly IDepartmentDataService departmentService;

        private readonly IStudentDataService studentService;

        public CoursesController(SchoolContext db, ICourseDataService courseService, IDepartmentDataService departmentService, IStudentDataService studentService)
        {
            this.db = db;
            this.courseService = courseService;
            this.departmentService = departmentService;
            this.studentService = studentService;
        }

        // GET: Course
        public async Task<ActionResult> Index(int? SelectedDepartment, int? studentId)
        {
            ViewBag.StudentId = studentId;

            var departments = await departmentService.GetDepartmentsAsync();
            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
            int departmentID = SelectedDepartment.GetValueOrDefault();

            IQueryable<Course> courses = db.Courses
                .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentID)
                .OrderBy(d => d.CourseID)
                .Include(d => d.Department);

            return View(courses.ToList());
        }

        // GET: Course/Details/5
        public async Task<ActionResult> Details(int? id, int? studentId)
        {
            ViewBag.StudentId = studentId;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CourseDetailsViewModel model = new CourseDetailsViewModel();

            model.Course = await courseService.GetCourseAsync(id.GetValueOrDefault());

            if (model.Course == null)
            {
                return HttpNotFound();
            }

            model.OtherCourses = await courseService.GetCourseRecommendationsAsync(model.Course.CourseID, model.Course.DepartmentID);

            return View(model);
        }

        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")]Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Courses.Add(course);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var courseToUpdate = db.Courses.Find(id);
            if (TryUpdateModel(courseToUpdate, "",
               new string[] { "Title", "Credits", "DepartmentID" }))
            {
                try
                {
                    db.SaveChanges();
                    courseService.DeleteCourseCache(id.GetValueOrDefault());

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        private async void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departments = await departmentService.GetDepartmentsAsync();

            ViewBag.DepartmentID = new SelectList(departments, "DepartmentID", "Name", selectedDepartment);
        }


        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
            {
                ViewBag.RowsAffected = db.Database.ExecuteSqlCommand("UPDATE Course SET Credits = Credits * {0}", multiplier);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Enroll(int courseId, int studentId)
        {
            if (ModelState.IsValid)
            {
                if (!db.Enrollments.Any(e => e.CourseID == courseId && e.StudentID == studentId))
                {
                    var newEnrollment = new Enrollment() { CourseID = courseId, StudentID = studentId };

                    db.Enrollments.Add(newEnrollment);
                    db.SaveChanges();

                    studentService.DeleteStudentSuggestedCoursesCache(studentId);

                    TempData["Success"] = $"Student successfully enrolled for Couse.";
                }
            }
            return RedirectToAction(nameof(Details), new { id = courseId, studentId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
