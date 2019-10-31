///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         GlucoseTrackerContext.cs
//	Description:       Adds Database Context for the GlucoseAPI Project
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Microsoft.EntityFrameworkCore;

namespace GlucoseAPI.Models.Entities
{
    /// <summary>
    /// Adds Database Context for the GlucoseAPI Project
    /// </summary>
    public partial class GlucoseTrackerContext : DbContext
    {
        /// <summary>
        /// Instantiates this extended context of DbContext and DbContext itself
        /// </summary>
        /// <param name="options">DbContextOptions for GlucoseTracker</param>
        public GlucoseTrackerContext(DbContextOptions<GlucoseTrackerContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DB Sets for all tables in the Database.
        /// </summary>
        public virtual DbSet<Doctor> Doctor { get; set; }
        public virtual DbSet<Auth> Auth{ get; set; }
        public virtual DbSet<TokenAuth> TokenAuth { get; set; }
        public virtual DbSet<MealItem> MealItem { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<PatientBloodSugar> PatientBloodSugar { get; set; }
        public virtual DbSet<PatientCarbohydrates> PatientCarbohydrates { get; set; }
        public virtual DbSet<PatientExercise> PatientExercise { get; set; }
    }
}
