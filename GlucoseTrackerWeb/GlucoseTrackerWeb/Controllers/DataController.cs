///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerWeb/GlucoseTrackerWeb
//	File Name:         DataController.cs
//	Description:       Controller for all Data.
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using GlucoseAPI.Models.Entities;
using GlucoseAPI.Services;
using GlucoseTrackerWeb.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GlucoseTrackerWeb.Controllers
{
    /// <summary>
    /// Controller for all Data.
    /// </summary>
    public class DataController : Controller
    {

        #region Dependency Injection
        IRepository<Patient> _patientRepo;
        IRepository<MealItem> _mealRepo;

        public DataController(IRepository<Patient> patientRepo, IRepository<MealItem> mealRepo)
        {
            _patientRepo = patientRepo;
            _mealRepo = mealRepo;

        }

        #endregion

        #region Patient Data
        /// <summary>
        /// Returns the Patient Data. For the Appropriate Patient 
        /// </summary>
        /// <param name="id">A Patient Id</param>
        /// <returns>The Patient Data View</returns>
        [HttpPost]
        public IActionResult Index([FromForm(Name = "id")] int id)
        {
            //The Doctor is logged in
            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                //Read the Patient.
                Patient patient = _patientRepo.Read(p => p.UserId == id, p => p.PatientBloodSugars, p => p.PatientCarbs, p => p.PatientExercises);

                //List of All Meals
                List<MealItem> meals = _mealRepo.ReadAll().ToList();

                //Order the Lists.
                patient.PatientBloodSugars = patient.PatientBloodSugars.OrderBy(bs => bs.TimeOfDay).ToList();
                patient.PatientCarbs = patient.PatientCarbs.OrderBy(pc => pc.TimeOfDay).ToList();
                patient.PatientExercises = patient.PatientExercises.OrderBy(pe => pe.TimeOfDay).ToList();

                //Key Value Pairs for JavaScript Chart.js
                Dictionary<DateTime, float> bloodSugarsBefore = new Dictionary<DateTime, float>();
                Dictionary<DateTime, float> bloodSugarsAfter = new Dictionary<DateTime, float>();
                Dictionary<DateTime, float> exercises = new Dictionary<DateTime, float>();
                Dictionary<DateTime, float> carbs = new Dictionary<DateTime, float>();

                //For each bloodSugar.
                foreach (var bloodSugar in patient.PatientBloodSugars)
                {
                    //Assign the Appropriate Meal.
                    bloodSugar.Meal = meals.FirstOrDefault(m => m.MealId == bloodSugar.MealId);

                    //Add the BloodSugars Appropriately to the Dictionary 
                    bloodSugarsBefore.Add(bloodSugar.TimeOfDay, bloodSugar.LevelBefore);
                    bloodSugarsAfter.Add(bloodSugar.TimeOfDay, bloodSugar.LevelAfter);
                }

                //For each carb.
                foreach (var carb in patient.PatientCarbs)
                {
                    //Assign the Appropriate Meal
                    carb.Meal = meals.FirstOrDefault(m => m.MealId == carb.MealId);

                    //Add the Carb Appropriately to the Dictionary
                    carbs.Add(carb.TimeOfDay, carb.FoodCarbs);
                }

                //For each exercise.
                foreach (var exercise in patient.PatientExercises)
                {
                    //Add the Exercise Appropriately to the Dictionary
                    exercises.Add(exercise.TimeOfDay, exercise.HoursExercised);
                }

                //Create a new PatientData View Model
                PatientDataVM patientDataVM = new PatientDataVM()
                {
                    FullName = $"{patient.LastName}, {patient.FirstName}",
                    PatientExercises = patient.PatientExercises,
                    PatientBloodSugars = patient.PatientBloodSugars,
                    PatientCarbohydrates = patient.PatientCarbs
                };

                //Assign ViewData for Chart.js
                ViewData["BloodSugarsBefore"] = JsonConvert.SerializeObject(bloodSugarsBefore);
                ViewData["BloodSugarsAfter"] = JsonConvert.SerializeObject(bloodSugarsAfter);
                ViewData["Exercises"] = JsonConvert.SerializeObject(exercises);
                ViewData["Carbs"] = JsonConvert.SerializeObject(carbs);

                //return the View with the Patient Data View Model
                return View(patientDataVM);
            }
            else
            {
                //Otherwise.. Redirect to Login
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}