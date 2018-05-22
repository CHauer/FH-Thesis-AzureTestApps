using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;

using ContosoUniversityFull.DAL;
using ContosoUniversityFull.Models;

namespace ContosoUniversityFull.Services
{
    public class CourseDataService : ICourseDataService
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly SchoolContext db;

        private MemoryCache cache;

        private readonly string AppVersion = "A";

        public CourseDataService(SchoolContext db)
        {
            this.db = db;
            cache = MemoryCache.Default;
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            // Define a unique key for entity.
            var key = $"{AppVersion}:Course:{id}";

            // Try to get the entity from the cache
            var value = (Course)cache.Get(key);

            if (value == null) // Cache miss
            {
                // If cache missed, get the entity from the original store and cache it.
                value = await db.Courses
                            .AsNoTracking()
                            .SingleOrDefaultAsync(c => c.CourseID == id)
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

        public async Task<List<Course>> GetCourseRecommendationsAsync(int courseId, int departmentId)
        {
            // Define a unique key for entity.
            var key = $"{AppVersion}:Course:{courseId}:Recommendations";

            // Try to get the entity from the cache
            var value = (List<Course>)cache.Get(key);

            if (value == null) // Cache miss
            {
                // If cache missed, get the entity from the original store and cache it.
                value = await db.Courses
                            .Where(c => c.CourseID != courseId && c.DepartmentID == departmentId)
                            .AsNoTracking()
                            .ToListAsync();

                // Avoid caching a null value.
                if (value != null)
                {
                    // Put the item in the cache with a expiration time of 3 minutes
                    cache.Set(key, value, DateTime.Now.AddMinutes(3));
                }
            }
            return value;
        }

        public void DeleteCourseCache(int id) => DeleteCacheKey($"{AppVersion}:Course:{id}");

        public void DeleteCourseRecommendationsCache(int id) => DeleteCacheKey($"{AppVersion}:Course:{id}:Recommendations");

        private void DeleteCacheKey(string key)
        {
            // Invalidate the current cache object
            cache.Remove(key);
        }
    }
}