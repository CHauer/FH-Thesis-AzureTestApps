using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using StackExchange.Redis;

using Newtonsoft.Json;
using ContosoUniversityCore.Data;
using ContosoUniversityCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace ContosoUniversityCore.Services
{
    public class RedisStudentDataService : IStudentDataService
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly SchoolContext db;

        /// <summary>
        /// The redis connection
        /// </summary>
        private IDistributedCache cache;

        private readonly string AppVersion = "A";

        public RedisStudentDataService(SchoolContext db, IDistributedCache cache)
        {
            this.db = db;
            this.cache = cache;
        }

        public async Task<Student> GetStudentAsync(int id)
        {
            var key = $"{AppVersion}:Student:{id}"; // Define a unique key for entity.

            // Try to get the entity from the cache.
            var data = await cache.GetAsync(key).ConfigureAwait(false);
            var json = Encoding.UTF8.GetString(data);

            var value = string.IsNullOrWhiteSpace(json)
                            ? default(Student) : JsonConvert.DeserializeObject<Student>(json);
            if (value == null) // Cache miss
            {
                // If cache missed, get the entity from the original store and cache it.
                value = await db.Students
                                  .Include(s => s.Enrollments)
                                  .Include(s => s.Enrollments.Select(e => e.Course))
                                  .AsNoTracking()
                                  .SingleOrDefaultAsync(m => m.ID == id);

                //value = await this.db.GetEntityAsync(entity).ConfigureAwait(false);

                if (value != null) // Avoid caching a null value.
                {
                    // Put the item in the cache with a expiration time of 3 minutes
                    var cacheEntryOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(3));

                    await cache.SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), cacheEntryOptions).ConfigureAwait(false);
                }
            }
            return value;
        }

        public async Task UpdateStudentAsync(Student entity, Picture userPicture)
        {
            // Update the object in the original data store - load student entity from db
            var studentToUpdate = db.Students.Find(entity.ID);

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
            await db.SaveChangesAsync();

            // Invalidate the current cache object
            var id = entity.ID;
            var key = $"{AppVersion}:Student:{id}"; // Get the correct key for the cached object.
            await cache.RemoveAsync(key).ConfigureAwait(false);
        }

        public async Task<List<Course>> GetStudentSuggestedCoursesAsync(int id)
        {
            // Define a unique key for entity.
            var key = $"{AppVersion}:Student:{id}:SuggestedCourses";

            // Try to get the entity from the cache.
            var data = await cache.GetAsync(key).ConfigureAwait(false);
            var json = Encoding.UTF8.GetString(data);

            var value = string.IsNullOrWhiteSpace(json)
                            ? default(List<Course>) : JsonConvert.DeserializeObject<List<Course>>(json);

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
                    var cacheEntryOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(3));

                    // Put the item in the cache with a expiration time of 3 minutes
                    await cache.SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), cacheEntryOptions).ConfigureAwait(false);
                }
            }
            return value;
        }

        public async Task<List<Department>> GetStudentDepartmentsAsync(int id)
        {
            // Define a unique key for entity.
            var key = $"{AppVersion}:Student:{id}:Departments";

            // Try to get the entity from the cache
            var data = await cache.GetAsync(key).ConfigureAwait(false);
            var json = Encoding.UTF8.GetString(data);

            var value = string.IsNullOrWhiteSpace(json)
                            ? default(List<Department>) : JsonConvert.DeserializeObject<List<Department>>(json);

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
                    var cacheEntryOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(3));

                    // Put the item in the cache with a expiration time of 3 minutes
                    await cache.SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), cacheEntryOptions).ConfigureAwait(false);
                }
            }
            return value;
        }

        public async void DeleteStudentCache(int id) => await DeleteCacheKey($"{AppVersion}: Student:{id}");

        public async void DeleteStudentSuggestedCoursesCache(int id) => await DeleteCacheKey($"{AppVersion}:Student:{id}:SuggestedCourses");

        public async void DeleteStudentDepartsmentsCache(int id) => await DeleteCacheKey($"{AppVersion}:Student:{id}:Departments");

        private async Task DeleteCacheKey(string key)
        {
            // Invalidate the current cache object
            await cache.RemoveAsync(key).ConfigureAwait(false);
        }
    }
}