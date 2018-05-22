using ContosoUniversityFull.DAL;
using ContosoUniversityFull.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;

namespace ContosoUniversityFull.Services
{
    public class DepartmentDataService : IDepartmentDataService
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly SchoolContext db;

        private MemoryCache cache;

        private readonly string AppVersion = "A";

        public DepartmentDataService(SchoolContext db)
        {
            this.db = db;
            cache = MemoryCache.Default;
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            // Define a unique key for entity.
            var key = $"{AppVersion}:Departments";

            // Try to get the entity from the cache
            var value = (List<Department>)cache.Get(key);

            if (value == null) // Cache miss
            {
                // If cache missed, get the entity from the original store and cache it
                value = await db.Departments.Include(d => d.Administrator)
                            .OrderBy(q => q.Name).ToListAsync().ConfigureAwait(false);

                // Avoid caching a null value.
                if (value != null)
                {
                    // Put the item in the cache with a expiration time of 3 minutes
                    cache.Set(key, value, DateTime.Now.AddMinutes(3));
                }
            }
            return value;
        }

        public void DeleteDepartmentsCache() => DeleteCacheKey($"{AppVersion}:Departments");

        private void DeleteCacheKey(string key)
        {
            // Invalidate the current cache object
            cache.Remove(key);
        }
    }
}