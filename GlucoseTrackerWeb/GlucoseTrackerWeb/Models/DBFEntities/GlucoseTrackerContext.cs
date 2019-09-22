using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GlucoseTrackerWeb.Models.DBFEntities
{
    public partial class GlucoseTrackerContext : DbContext
    {
        public GlucoseTrackerContext()
        {
        }

        public GlucoseTrackerContext(DbContextOptions<GlucoseTrackerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Doctor> Doctor { get; set; }
        public virtual DbSet<MealItem> MealItem { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<PatientBloodSugar> PatientBloodSugar { get; set; }
        public virtual DbSet<PatientCarbohydrates> PatientCarbohydrates { get; set; }
        public virtual DbSet<PatientExercise> PatientExercise { get; set; }
        public virtual DbSet<User> User { get; set; }

        // Unable to generate entity type for table 'PatientCarbohydrates_MealItem'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.Property(e => e.DoctorId)
                    .HasColumnName("DoctorID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NumberOfPatients).HasColumnType("int(11)");

                entity.HasOne(d => d.DoctorNavigation)
                    .WithOne(p => p.Doctor)
                    .HasForeignKey<Doctor>(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DoctorID_fk");
            });

            modelBuilder.Entity<MealItem>(entity =>
            {
                entity.HasKey(e => e.FoodId)
                    .HasName("PRIMARY");

                entity.Property(e => e.FoodId)
                    .HasColumnName("FoodID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Carbs).HasColumnType("int(11)");

                entity.Property(e => e.FoodName)
                    .IsRequired()
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasIndex(e => e.DoctorId)
                    .HasName("PatientDoctorID_fk");

                entity.Property(e => e.PatientId)
                    .HasColumnName("PatientID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DoctorId)
                    .HasColumnName("DoctorID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.Patient)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PatientDoctorID_fk");

                entity.HasOne(d => d.PatientNavigation)
                    .WithOne(p => p.Patient)
                    .HasForeignKey<Patient>(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PatientID_fk");
            });

            modelBuilder.Entity<PatientBloodSugar>(entity =>
            {
                entity.HasKey(e => e.PatientId)
                    .HasName("PRIMARY");

                entity.Property(e => e.PatientId)
                    .HasColumnName("PatientID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Meal)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.TimeOfDay).HasColumnType("datetime");

                entity.HasOne(d => d.Patient)
                    .WithOne(p => p.PatientBloodSugar)
                    .HasForeignKey<PatientBloodSugar>(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PatientBloodSugarID_fk");
            });

            modelBuilder.Entity<PatientCarbohydrates>(entity =>
            {
                entity.HasKey(e => e.PatientId)
                    .HasName("PRIMARY");

                entity.Property(e => e.PatientId)
                    .HasColumnName("PatientID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FoodCarbs).HasColumnType("int(11)");

                entity.Property(e => e.Meal)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.MealName)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.TimeOfDay).HasColumnType("datetime");

                entity.Property(e => e.TotalCarbs).HasColumnType("int(11)");

                entity.HasOne(d => d.Patient)
                    .WithOne(p => p.PatientCarbohydrates)
                    .HasForeignKey<PatientCarbohydrates>(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PatientCarbohydrates_Patient_PatientID_fk");
            });

            modelBuilder.Entity<PatientExercise>(entity =>
            {
                entity.HasKey(e => e.PatientId)
                    .HasName("PRIMARY");

                entity.Property(e => e.PatientId)
                    .HasColumnName("PatientID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HoursExercised).HasColumnType("int(11)");

                entity.Property(e => e.TimeOfDay).HasColumnType("datetime");

                entity.HasOne(d => d.Patient)
                    .WithOne(p => p.PatientExercise)
                    .HasForeignKey<PatientExercise>(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PatientExerciseID_fk");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Email).HasColumnType("varchar(255)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.MiddleName)
                    .IsRequired()
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PhoneNumber).HasColumnType("varchar(11)");
            });
        }
    }
}
