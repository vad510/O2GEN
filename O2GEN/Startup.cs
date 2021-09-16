using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using O2GEN.Models;
using O2GEN.Authorization;
using System;

namespace O2GEN
{

        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                //services.AddDbContext<ApplicationContext>(options =>
                //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

                //services.AddIdentity<User, IdentityRole>()
                //    .AddEntityFrameworkStores<ApplicationContext>();

                services.AddControllersWithViews();

                services.AddMemoryCache();
                services.AddSession(options => {
                    options.IdleTimeout = TimeSpan.FromMinutes(60);
                });

            // todo cookies for default login path
        }

            public void Configure(IApplicationBuilder app)
            {
                app.UseSession();
                DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
                defaultFilesOptions.DefaultFileNames.Clear();
                defaultFilesOptions.DefaultFileNames.Add("index.html");
                app.UseDefaultFiles(defaultFilesOptions);
                app.UseDeveloperExceptionPage();

                //app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseMiddleware<JwtMiddleware>();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });
        }
        }
    }
