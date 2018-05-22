using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using PagedList;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Threading.Tasks;

using ContosoUniversityFull.DAL;
using ContosoUniversityFull.Models;
using ContosoUniversityFull.Models.SchoolViewModels;
using ContosoUniversityFull.Services;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace ContosoUniversityFull.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext db;

        private readonly IStudentDataService studentService;

        private readonly IPictureDataService pictureService;

        public StudentsController(SchoolContext db, IStudentDataService studentService, IPictureDataService pictureService)
        {
            this.db = db;
            this.studentService = studentService;
            this.pictureService = pictureService;
        }

        // GET: Student
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = from s in db.Students
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
                default:  // Name ascending 
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Student/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Student student = await studentService.GetStudentAsync(id.GetValueOrDefault());

            if (student == null)
            {
                return HttpNotFound();
            }

            ViewBag.StudentId = id;
            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName, FirstMidName, EnrollmentDate")]Student student, HttpPostedFileBase picture)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userPicture = HandleUserPictureUpload(picture);

                    if (userPicture != null)
                    {
                        student.UserPicture = userPicture;
                    }

                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        private Picture HandleUserPictureUpload(HttpPostedFileBase picture)
        {
            if (picture != null && picture.ContentLength > 0)
            {
                var userPicture = new Picture()
                {
                    ContentType = picture.ContentType
                };

                using (var reader = new BinaryReader(picture.InputStream))
                {
                    userPicture.OriginalData = reader.ReadBytes(picture.ContentLength);
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

        // GET: Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(Student model, HttpPostedFileBase picture)
        {
            var userPicture = HandleUserPictureUpload(picture);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await studentService.UpdateStudentAsync(model, userPicture);

                return RedirectToAction(nameof(Details), new { model.ID });
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(model);
        }

        // GET: Student/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Dashboard(int id)
        {
            StudentDashboardViewModel model = new StudentDashboardViewModel();

            ViewBag.StudentId = id;

            model.Student = await studentService.GetStudentAsync(id);

            if (model.Student == null)
            {
                return new HttpNotFoundResult();
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
                return File("/Content/UserImage.png", "image/png");
            }

            var picture = await pictureService.GetPictureAsync(id.GetValueOrDefault());

            if (picture == null || picture.Data.Length == 0)
            {
                return File("/Content/UserImage.png", "image/png");
            }

            return File(picture.Data, "image/jpg");
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
