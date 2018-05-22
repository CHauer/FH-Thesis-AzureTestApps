using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContosoUniversityCore.Data;
using ContosoUniversityCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ContosoUniversityCore.Services
{
    public class PictureDataService : IPictureDataService
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly SchoolContext db;

        private IMemoryCache cache;

        private readonly string AppVersion = "A";

        public PictureDataService(SchoolContext db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
        }

        public async Task<Picture> GetPictureAsync(int id)
        {
            // Define a unique key for entity.
            var key = $"{AppVersion}:Picture:{id}";

            // Try to get the entity from the cache
            var value = (Picture)cache.Get(key);

            if (value == null) // Cache miss
            {
                // If cache missed, get the entity from the original store and cache it.
                value = await db.Pictures.FirstOrDefaultAsync(p => p.PictureID == id).ConfigureAwait(false);

                // Avoid caching a null value.
                if (value != null)
                {
                    // Put the item in the cache with a expiration time of 3 minutes
                    cache.Set(key, value, DateTime.Now.AddMinutes(3));
                }
            }
            return value;
        }

        public void DeletePictureCache(int id) => DeleteCacheKey($"{AppVersion}:Picture:{id}");

        private void DeleteCacheKey(string key)
        {
            // Invalidate the current cache object
            cache.Remove(key);
        }

    }
}