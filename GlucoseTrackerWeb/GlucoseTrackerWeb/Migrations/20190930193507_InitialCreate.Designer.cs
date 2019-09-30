﻿// <auto-generated />
using System;
using GlucoseTrackerWeb.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GlucoseTrackerWeb.Migrations
{
    [DbContext(typeof(GlucoseTrackerContext))]
    [Migration("20190930193507_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.Login", b =>
                {
                    b.Property<int>("LoginId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Password");

                    b.Property<int>("UserId");

                    b.HasKey("LoginId");

                    b.HasIndex("UserId");

                    b.ToTable("Login");
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.MealItem", b =>
                {
                    b.Property<int>("MealId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Carbs");

                    b.Property<int>("FoodId");

                    b.Property<string>("FoodName");

                    b.HasKey("MealId");

                    b.ToTable("MealItem");
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.PatientBloodSugar", b =>
                {
                    b.Property<int>("BloodId")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("LevelAfter");

                    b.Property<float>("LevelBefore");

                    b.Property<string>("Meal");

                    b.Property<int>("PatientId");

                    b.Property<DateTime?>("TimeOfDay");

                    b.HasKey("BloodId");

                    b.HasIndex("PatientId")
                        .IsUnique();

                    b.ToTable("PatientBloodSugar");
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.PatientCarbohydrates", b =>
                {
                    b.Property<int>("CarbId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FoodCarbs");

                    b.Property<string>("Meal");

                    b.Property<string>("MealName");

                    b.Property<int>("PatientId");

                    b.Property<DateTime?>("TimeOfDay");

                    b.Property<int>("TotalCarbs");

                    b.HasKey("CarbId");

                    b.HasIndex("PatientId")
                        .IsUnique();

                    b.ToTable("PatientCarbohydrates");
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.PatientExercise", b =>
                {
                    b.Property<int>("ExerciseId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("HoursExercised");

                    b.Property<int>("PatientId");

                    b.Property<DateTime?>("TimeOfDay");

                    b.HasKey("ExerciseId");

                    b.HasIndex("PatientId")
                        .IsUnique();

                    b.ToTable("PatientExercise");
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("MiddleName");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.HasKey("UserId");

                    b.ToTable("User");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.Doctor", b =>
                {
                    b.HasBaseType("GlucoseTrackerWeb.Models.Entities.User");

                    b.Property<int>("DoctorId");

                    b.Property<int>("NumberOfPatients");

                    b.HasDiscriminator().HasValue("Doctor");
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.Patient", b =>
                {
                    b.HasBaseType("GlucoseTrackerWeb.Models.Entities.User");

                    b.Property<int>("DoctorId")
                        .HasColumnName("Patient_DoctorId");

                    b.Property<int>("PatientId");

                    b.HasIndex("DoctorId");

                    b.HasDiscriminator().HasValue("Patient");
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.Login", b =>
                {
                    b.HasOne("GlucoseTrackerWeb.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.PatientBloodSugar", b =>
                {
                    b.HasOne("GlucoseTrackerWeb.Models.Entities.Patient", "Patient")
                        .WithOne("PatientBloodSugar")
                        .HasForeignKey("GlucoseTrackerWeb.Models.Entities.PatientBloodSugar", "PatientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.PatientCarbohydrates", b =>
                {
                    b.HasOne("GlucoseTrackerWeb.Models.Entities.Patient", "Patient")
                        .WithOne("PatientCarbohydrates")
                        .HasForeignKey("GlucoseTrackerWeb.Models.Entities.PatientCarbohydrates", "PatientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.PatientExercise", b =>
                {
                    b.HasOne("GlucoseTrackerWeb.Models.Entities.Patient", "Patient")
                        .WithOne("PatientExercise")
                        .HasForeignKey("GlucoseTrackerWeb.Models.Entities.PatientExercise", "PatientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GlucoseTrackerWeb.Models.Entities.Patient", b =>
                {
                    b.HasOne("GlucoseTrackerWeb.Models.Entities.Doctor", "Doctor")
                        .WithMany("Patient")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
