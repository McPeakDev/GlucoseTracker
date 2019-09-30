using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GlucoseTrackerWeb.Models.Entities
{
    public partial class GlucoseTrackerContext : DbContext
    {
        public GlucoseTrackerContext(DbContextOptions<GlucoseTrackerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Doctor> Doctor { get; set; }
        public virtual DbSet<Login> Login { get; set; }
        public virtual DbSet<MealItem> MealItem { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<PatientBloodSugar> PatientBloodSugar { get; set; }
        public virtual DbSet<PatientCarbohydrates> PatientCarbohydrates { get; set; }
        public virtual DbSet<PatientExercise> PatientExercise { get; set; }
        public virtual DbSet<User> User { get; set; }

    }
}
