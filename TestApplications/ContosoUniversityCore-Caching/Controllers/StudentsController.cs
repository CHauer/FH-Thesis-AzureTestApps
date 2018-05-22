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
using ContosoUniversityCore.Services;

using Microsoft.AspNetCore.Http;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace ContosoUniversityCore.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext context;

        private readonly IStudentDataService studentService;

        private readonly IPictureDataService pictureService;

        public StudentsController(SchoolContext context, IStudentDataService studentService, IPictureDataService pictureService)
        {
            this.context = context;
            this.studentService = studentService;
            this.pictureService = pictureService;
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

            var students = from s in context.Students
                           select s;
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

            Student student = await studentService.GetStudentAsync(id.GetValueOrDefault());

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
                    var userPicture = await HandleUserPictureUpload(picture);

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

        private async Task<Picture> HandleUserPictureUpload(IFormFile picture)
        {
            if (picture != null && picture.Length > 0)
            {
                var userPicture = new Picture()
                {
                    ContentType = picture.ContentType
                };

                using (var memoryStream = new MemoryStream())
                {
                    await picture.CopyToAsync(memoryStream);
                    userPicture.OriginalData = memoryStream.ToArray();
                }
                userPicture.Data = GeneratePicture(userPicture.OriginalData, 250, 350);
                userPicture.ThumbnailData = GeneratePicture(userPicture.OriginalData, 50, 50);

                return userPicture;
            }

            return null;
        }

        private byte[] GeneratePicture(byte[] inputData, int width, int height)
        {
            using (Image<Rgba32> image = Image.Load(inputData))
            {
                image.Mutate(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Crop, Size = new Size(width, height) }));

                using (var memoryStream = new MemoryStream())
                {
                    image.SaveAsJpeg(memoryStream);
                    return memoryStream.ToArray();
                }
            }

        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await context.Students.SingleOrDefaultAsync(m => m.ID == id);
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
        public async Task<IActionResult> EditPost(Student model, IFormFile picture)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userPicture = await HandleUserPictureUpload(picture);

            try
            {
                await studentService.UpdateStudentAsync(model, userPicture);

                return RedirectToAction(nameof(Details), new { model.ID });
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
            }
            return View(model);
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

            ViewBag.StudentId = id;

            model.Student = await studentService.GetStudentAsync(id);

            if (model.Student == null)
            {
                return NotFound();
            }

            model.SuggestedCourses = await studentService.GetStudentSuggestedCoursesAsync(id);
            model.StudentDepartments = await studentService.GetStudentDepartmentsAsync(id);


            return View(model);
        }

        [ActionName("UserPicture")]
        public async Task<FileResult> GetUserPicture(int? id)
        {
            if (id == null)
            {
                return File("/images/UserImage.png", "image/png");
            }

            var picture = await pictureService.GetPictureAsync(id.GetValueOrDefault());

            if (picture == null || picture.Data.Length == 0)
            {
                return File("/images/UserImage.png", "image/png");
            }

            return File(picture.Data, "image/jpg");
        }
    }
}
