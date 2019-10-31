///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         Startup.cs
//	Description:       Loads Needed Information for GlucoseAPI
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using GlucoseAPI.Models.Entities;
using GlucoseAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlucoseAPI
{
    /// <summary>
    /// Loads Needed Information for GlucoseAPI
    /// </summary>
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options
                .SerializerSettings
                .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //Add GlucoseTracker Context
            services.AddDbContext<GlucoseTrackerContext>(options => options
            .UseMySql(Configuration.GetConnectionString("DevConnection")));


            //Scope Out all Tables need for Patients
            services.AddScoped<IRepository<Auth>, GlucoseDbRepository<Auth>>();
            services.AddScoped<IRepository<TokenAuth>, GlucoseDbRepository<TokenAuth>>();
            services.AddScoped<IRepository<Patient>,GlucoseDbRepository<Patient>>();
            services.AddScoped<IRepository<Doctor>, GlucoseDbRepository<Doctor>>();
            services.AddScoped<IRepository<PatientBloodSugar>, GlucoseDbRepository<PatientBloodSugar>>();
            services.AddScoped<IRepository<PatientCarbohydrates>, GlucoseDbRepository<PatientCarbohydrates>>();
            services.AddScoped<IRepository<PatientExercise>, GlucoseDbRepository<PatientExercise>>();
            services.AddScoped<IRepository<MealItem>, GlucoseDbRepository<MealItem>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
