﻿// <auto-generated />
using System;
using GlucoseAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GlucoseAPI.Migrations
{
    [DbContext(typeof(GlucoseTrackerContext))]
    [Migration("20191015075404_GlucoseTracker")]
    partial class GlucoseTracker
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("GlucoseAPI.Models.Entities.Login", b =>
                {
                    b.Property<int>("LoginId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("UserId");

                    b.HasKey("LoginId");

                    b.HasIndex("UserId");

                    b.ToTable("Login");
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.MealItem", b =>
                {
                    b.Property<int>("MealId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Carbs");

                    b.Property<string>("FoodName");

                    b.HasKey("MealId");

                    b.ToTable("MealItem");
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.PatientBloodSugar", b =>
                {
                    b.Property<int>("BloodId")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("LevelAfter");

                    b.Property<float>("LevelBefore");

                    b.Property<int?>("MealId");

                    b.Property<int>("PatientId");

                    b.Property<DateTime?>("TimeOfDay");

                    b.HasKey("BloodId");

                    b.HasIndex("MealId");

                    b.HasIndex("PatientId")
                        .IsUnique();

                    b.ToTable("PatientBloodSugar");
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.PatientCarbohydrates", b =>
                {
                    b.Property<int>("CarbId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FoodCarbs");

                    b.Property<int?>("MealId");

                    b.Property<int>("PatientId");

                    b.Property<DateTime?>("TimeOfDay");

                    b.Property<int>("TotalCarbs");

                    b.HasKey("CarbId");

                    b.HasIndex("MealId");

                    b.HasIndex("PatientId")
                        .IsUnique();

                    b.ToTable("PatientCarbohydrates");
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.PatientExercise", b =>
                {
                    b.Property<int>("ExerciseId")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("HoursExercised");

                    b.Property<int>("PatientId");

                    b.Property<DateTime?>("TimeOfDay");

                    b.HasKey("ExerciseId");

                    b.HasIndex("PatientId")
                        .IsUnique();

                    b.ToTable("PatientExercise");
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<string>("MiddleName")
                        .HasMaxLength(150);

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(11);

                    b.HasKey("UserId");

                    b.ToTable("User");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.Doctor", b =>
                {
                    b.HasBaseType("GlucoseAPI.Models.Entities.User");

                    b.HasDiscriminator().HasValue("Doctor");
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.Patient", b =>
                {
                    b.HasBaseType("GlucoseAPI.Models.Entities.User");

                    b.Property<int?>("DoctorId");

                    b.HasIndex("DoctorId");

                    b.HasDiscriminator().HasValue("Patient");
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.Login", b =>
                {
                    b.HasOne("GlucoseAPI.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.PatientBloodSugar", b =>
                {
                    b.HasOne("GlucoseAPI.Models.Entities.MealItem", "Meal")
                        .WithMany()
                        .HasForeignKey("MealId");

                    b.HasOne("GlucoseAPI.Models.Entities.Patient", "Patient")
                        .WithOne("PatientBloodSugar")
                        .HasForeignKey("GlucoseAPI.Models.Entities.PatientBloodSugar", "PatientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.PatientCarbohydrates", b =>
                {
                    b.HasOne("GlucoseAPI.Models.Entities.MealItem", "Meal")
                        .WithMany()
                        .HasForeignKey("MealId");

                    b.HasOne("GlucoseAPI.Models.Entities.Patient", "Patient")
                        .WithOne("PatientCarbohydrates")
                        .HasForeignKey("GlucoseAPI.Models.Entities.PatientCarbohydrates", "PatientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.PatientExercise", b =>
                {
                    b.HasOne("GlucoseAPI.Models.Entities.Patient", "Patient")
                        .WithOne("PatientExercise")
                        .HasForeignKey("GlucoseAPI.Models.Entities.PatientExercise", "PatientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GlucoseAPI.Models.Entities.Patient", b =>
                {
                    b.HasOne("GlucoseAPI.Models.Entities.Doctor", "Doctor")
                        .WithMany("Patients")
                        .HasForeignKey("DoctorId");
                });
#pragma warning restore 612, 618
        }
    }
}
