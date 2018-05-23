using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using ContosoUniversityCore.Data;
using ContosoUniversityCore.Models;
using ContosoUniversityCore.Models.SchoolViewModels;

using Microsoft.AspNetCore.Http;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;
using ContosoUniversityCore.Services;

namespace ContosoUniversityCore.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext context;

        private readonly IUserPictureService userPictureService;

        public StudentsController(SchoolContext context, IUserPictureService userPictureService)
        {
            this.context = context;
            this.userPictureService = userPictureService;
        }

        // GET: Students
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var students = context.Students.Include(s => s.UserPicture).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 100;
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }
        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await context.Students.Include(s => s.UserPicture)
                              .Include(s => s.Enrollments)
                              .ThenInclude(e => e.Course)
                              .AsNoTracking()
                              .SingleOrDefaultAsync(m => m.ID == id);

            if (student == null)
            {
                return NotFound();
            }

            ViewBag.StudentId = id;
            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("EnrollmentDate,FirstMidName,LastName")] Student student, IFormFile picture)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userPicture = await userPictureService.CreateUserPictureAsync(picture);

                    if (userPicture != null)
                    {
                        student.UserPicture = userPicture;

                    }

                    context.Add(student);
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await context.Students.Include(s => s.UserPicture).SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, IFormFile picture)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await context.Students.SingleOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "",
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
            {

                var userPicture = await userPictureService.CreateUserPictureAsync(picture);

                if (userPicture != null)
                {
                    studentToUpdate.UserPicture = userPicture;
                }

                try
                {
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }
        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await context.Students
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await context.Students
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                context.Students.Remove(student);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool StudentExists(int id)
        {
            return context.Students.Any(e => e.ID == id);
        }

        public async Task<IActionResult> Dashboard(int id)
        {
            StudentDashboardViewModel model = new StudentDashboardViewModel();

            model.Student = await context.Students.Include(s => s.UserPicture)
                                .Include(s => s.Enrollments)
                                .ThenInclude(e => e.Course)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);

            ViewBag.StudentId = id;

            model.SuggestedCourses = context.Courses
                .Where(c => c.Enrollments.All(e => e.StudentID != id))
                .OrderBy(c => c.Enrollments.Count)
                .Take(10)
                .AsNoTracking().ToList();

            var departmentIds = context.Enrollments
                .Where(e => e.StudentID == id)
                .Select(e => e.Course.DepartmentID).Distinct();

            model.StudentDepartments = context.Departments.Where(d => departmentIds.Contains(d.DepartmentID)).AsNoTracking().ToList();

            if (model.Student == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [ActionName("UserPicture")]
        [Obsolete("Use Urls in Picture Model")]
        public IActionResult GetUserPicture(int? id)
        {
            if (id == null)
            {
                return File("/images/UserImage.png", "image/png");
            }

            var picture = context.Pictures.FirstOrDefault(p => p.PictureID == id);

            if (picture == null || string.IsNullOrEmpty(picture.PictureUrl))
            {
                return File("/images/UserImage.png", "image/png");
            }

            return new RedirectResult(picture.PictureUrl);
        }
    }
}
