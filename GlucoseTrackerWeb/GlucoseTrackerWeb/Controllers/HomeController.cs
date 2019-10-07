using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GlucoseTrackerWeb.Models;
using GlucoseTrackerWeb.Services;
using GlucoseTrackerWeb.Models.Entities;
using static BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Http;
using SessionExtensions = GlucoseTrackerWeb.Services.SessionExtensions;

namespace GlucoseTrackerWeb.Controllers
{
    public class HomeController : Controller
    {
        private IDbRepository<Doctor> _doctorRepo;
        private IDbRepository<Patient> _patientRepo;

        private static ISession _session;

        public HomeController(IDbRepository<Doctor> doctorRepo, IDbRepository<Patient> patientRepo)
        {
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
        }

        [HttpPost]
        public IActionResult Login(Credentials creds)
        {
            Doctor doctor = _doctorRepo.Read(creds.Email);

            if (Verify(creds.Password, doctor.Password))
            {
                _session = HttpContext.Session;
                SessionExtensions.SetBool(_session, "LoggedIn", true);
                return RedirectToAction("Dashboard", doctor);
            }

            TempData["BadLogin"] = true;
            return RedirectToAction("Index");

        }
        public IActionResult Logout()
        {
            _session = null;
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            if (SessionExtensions.GetBool(_session, "LoggedIn"))
            {
                return RedirectToAction("Dashboard");

            }
            else
            {
                return View();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _doctorRepo.Create(doctor);
                return RedirectToAction("Index");
            }
            return View(doctor);
        }
        public IActionResult Dashboard(Doctor doctor)
        {
            var model = _patientRepo.ReadAll(doctor.UserId);

            if (SessionExtensions.GetBool(_session, "LoggedIn"))
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
