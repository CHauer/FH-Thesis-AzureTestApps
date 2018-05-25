using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ContosoUniversityCore.Data;

using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using ContosoUniversityCore.Services;
using System.Data.SqlClient;

namespace ContosoUniversityCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var conn = new SqlConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"))
            {
                ConnectRetryCount = 5,
                ConnectRetryInterval = 2,
                MaxPoolSize = 900,
                MinPoolSize = 5
            };

            services.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(conn.ToString(), sqloptions => sqloptions.EnableRetryOnFailure()));

            services.AddMemoryCache();
            services.AddMvc();

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("RedisCache");
            });

            services.AddTransient<IPictureDataService, PictureDataService>();
            services.AddTransient<IDepartmentDataService, DepartmentDataService>();
            services.AddTransient<ICourseDataService, CourseDataService>();
            services.AddTransient<IStudentDataService, LocalStudentDataService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
