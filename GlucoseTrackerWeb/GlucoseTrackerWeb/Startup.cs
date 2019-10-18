using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlucoseAPI.Models.Entities;
using GlucoseTrackerWeb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProMan.Services;

namespace GlucoseTrackerWeb
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<GlucoseTrackerContext>(options => options.UseMySql(Configuration.GetConnectionString("DevConnection")));
            services.AddScoped<IRepository<Doctor>, GlucoseDbRepository<Doctor>>();
            services.AddScoped<IRepository<Patient>, GlucoseDbRepository<Patient>>();
            services.AddScoped<IRepository<Credentials>, GlucoseDbRepository<Credentials>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "default",
                    template: "/",
                    defaults: new {Controller = "Home", Action = "Index"} );

                routes.MapRoute(
                    name: "dashboard",
                    template: "Dashboard",
                    defaults: new { Controller = "Home", Action = "Dashboard" });

                routes.MapRoute(
                    name: "create",
                    template: "Create",
                    defaults: new { Controller = "Home", Action = "Create" });

                routes.MapRoute(
                    name: "login",
                    template: "Login",
                    defaults: new { Controller = "Home", Action = "Login" });


            });
        }
    }
}
