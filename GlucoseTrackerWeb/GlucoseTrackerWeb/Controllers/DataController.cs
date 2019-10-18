using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlucoseAPI.Models.Entities;
using GlucoseAPI.Services;
using GlucoseTrackerWeb.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

            foreach (var bloodSugar in patient.PatientBloodSugars)
            {
                bloodSugar.Meal = meals.FirstOrDefault(m => m.MealId == bloodSugar.MealId);
            }

            foreach (var carb in patient.PatientCarbs)
            {
                carb.Meal = meals.FirstOrDefault(m => m.MealId == carb.MealId);
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

                return View(patientDataVM);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}