using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GlucoseAPI.Models.Entities;
using GlucoseAPI.Services;
using GlucoseTrackerWeb.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GlucoseTrackerWeb.Controllers
{
    public class DataController : Controller
    {
        IRepository<Patient> _patientRepo;
        IRepository<MealItem> _mealRepo;

        public DataController(IRepository<Patient> patientRepo, IRepository<MealItem> mealRepo)
        {
            _patientRepo = patientRepo;
            _mealRepo = mealRepo;

        }
        [HttpPost]
        public IActionResult Index([FromForm(Name = "id")] int id)
        {
            Patient patient = _patientRepo.Read(p => p.UserId == id, p => p.PatientBloodSugars, p => p.PatientCarbs, p => p.PatientExercises);
            List<MealItem> meals = _mealRepo.ReadAll().ToList();

            Dictionary<DateTime, float> bloodSugarsBefore = new Dictionary<DateTime, float>();
            Dictionary<DateTime, float> bloodSugarsAfter = new Dictionary<DateTime, float>();
            Dictionary<DateTime, float> exercises = new Dictionary<DateTime, float>();
            Dictionary<DateTime, float> carbs = new Dictionary<DateTime, float>();

            foreach (var bloodSugar in patient.PatientBloodSugars)
            {
                bloodSugar.Meal = meals.FirstOrDefault(m => m.MealId == bloodSugar.MealId);
                bloodSugarsBefore.Add(bloodSugar.TimeOfDay, bloodSugar.LevelBefore);
                bloodSugarsAfter.Add(bloodSugar.TimeOfDay, bloodSugar.LevelAfter);
            }

            foreach (var carb in patient.PatientCarbs)
            {
                carb.Meal = meals.FirstOrDefault(m => m.MealId == carb.MealId);
                carbs.Add(carb.TimeOfDay, carb.FoodCarbs);
            }

            foreach (var exercise in patient.PatientExercises)
            {
                exercises.Add(exercise.TimeOfDay, exercise.HoursExercised);
            }

            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                PatientDataVM patientDataVM = new PatientDataVM()
                {
                    FullName = $"{patient.LastName}, {patient.FirstName}",
                    PatientExercises = patient.PatientExercises,
                    PatientBloodSugars = patient.PatientBloodSugars,
                    PatientCarbohydrates = patient.PatientCarbs
                };

                ViewData["BloodSugarsBefore"] = JsonConvert.SerializeObject(bloodSugarsBefore);
                ViewData["BloodSugarsAfter"] = JsonConvert.SerializeObject(bloodSugarsAfter);
                ViewData["Exercises"] = JsonConvert.SerializeObject(exercises);
                ViewData["Carbs"] = JsonConvert.SerializeObject(carbs);

                return View(patientDataVM);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}