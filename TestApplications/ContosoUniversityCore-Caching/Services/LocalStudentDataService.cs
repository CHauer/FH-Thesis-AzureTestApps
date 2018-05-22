using ContosoUniversityCore.Data;
using ContosoUniversityCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ContosoUniversityCore.Services
{
    public class LocalStudentDataService : IStudentDataService
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly SchoolContext db;

        private IMemoryCache cache;


        private readonly string AppVersion = "A";

        public LocalStudentDataService(SchoolContext db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
        }

        public async Task<Student> GetStudentAsync(int id)
        {
            // Define a unique key for entity.
            var key = $"{AppVersion}:Student:{id}";

            // Try to get the entity from the cache
            var value = (Student)cache.Get(key);

            if (value == null) // Cache miss
            {
                // If cache missed, get the entity from the original store and cache it.
                value = await db.Students
                            .Include(s => s.Enrollments)
                            .ThenInclude(e => e.Course)
                                  .AsNoTracking()
                                  .SingleOrDefaultAsync(m => m.ID == id)
                            .ConfigureAwait(false);

                // Avoid caching a null value.
                if (value != null)
                {
                    // Put the item in the cache with a expiration time of 3 minutes
                    cache.Set(key, value, DateTime.Now.AddMinutes(3));
                }
            }
            return value;
        }

        public async Task<List<Course>> GetStudentSuggestedCoursesAsync(int id)
        {
            // Define a unique key for entity.
            var key = $"{AppVersion}:Student:{id}:SuggestedCourses";

            // Try to get the entity from the cache
            var value = (List<Course>)cache.Get(key);

            if (value == null) // Cache miss
            {
                // If cache missed, get the entity from the original store and cache it.
                value = await db.Courses
                            .Where(c => c.Enrollments.All(e => e.StudentID != id))
                            .OrderBy(c => c.Enrollments.Count)
                            .Take(10)
                            .AsNoTracking().ToListAsync().ConfigureAwait(false);

                // Avoid caching a null value.
                if (value != null)
                {
                    // Put the item in the cache with a expiration time of 3 minutes
                    cache.Set(key, value, DateTime.Now.AddMinutes(3));
                }
            }
            return value;
        }

        public async Task<List<Department>> GetStudentDepartmentsAsync(int id)
        {
            // Define a unique key for entity.
            var key = $"{AppVersion}:Student:{id}:Departments";

            // Try to get the entity from the cache
            var value = (List<Department>)cache.Get(key);

            if (value == null) // Cache miss
            {
                // If cache missed, get the entity from the original store and cache it.
                IQueryable<int> departmentIds = db.Enrollments.Where(e => e.StudentID == id).Select(e => e.Course.DepartmentID);

                value = await db.Departments
                    .Where(d => departmentIds.Contains(d.DepartmentID))
                    .AsNoTracking().ToListAsync();

                // Avoid caching a null value.
                if (value != null)
                {
                    // Put the item in the cache with a expiration time of 3 minutes
                    cache.Set(key, value, DateTime.Now.AddMinutes(3));
                }
            }
            return value;
        }

        public async Task UpdateStudentAsync(Student entity, Picture userPicture)
        {
            // Update the object in the original data store - load student entity from db
            var studentToUpdate = await db.Students.FindAsync(entity.ID).ConfigureAwait(false);

            // updated values
            studentToUpdate.LastName = entity.LastName;
            studentToUpdate.FirstMidName = entity.FirstMidName;
            studentToUpdate.EnrollmentDate = entity.EnrollmentDate;

            // if new picture provided - update
            if (userPicture != null)
            {
                studentToUpdate.UserPicture = userPicture;
            }

            // save changed to db
            await db.SaveChangesAsync().ConfigureAwait(false);

            // Invalidate the current cache object
            var id = entity.ID;
            var key = $"{AppVersion}:Student:{id}"; // Get the correct key for the cached object.
            cache.Remove(key);
        }

        public void DeleteStudentCache(int id) => DeleteCacheKey($"{AppVersion}:Student:{id}");

        public void DeleteStudentSuggestedCoursesCache(int id) => DeleteCacheKey($"{AppVersion}:Student:{id}:SuggestedCourses");

        public void DeleteStudentDepartsmentsCache(int id) => DeleteCacheKey($"{AppVersion}:Student:{id}:Departments");

        private void DeleteCacheKey(string key)
        {
            // Invalidate the current cache object
            cache.Remove(key);
        }
    }
}