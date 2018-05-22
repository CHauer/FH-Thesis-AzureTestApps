using ContosoUniversityCore.Data;
using ContosoUniversityCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ContosoUniversityCore.Services
{
    public class DepartmentDataService : IDepartmentDataService
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly SchoolContext db;

        private IMemoryCache cache;

        private readonly string AppVersion = "A";

        public DepartmentDataService(SchoolContext db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
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