using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GlucoseTrackerWeb.Models;
using GlucoseTrackerWeb.Services;
using GlucoseAPI.Models.Entities;
using static BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Http;
using SessionExtensions = GlucoseTrackerWeb.Services.SessionExtensions;
using ProMan.Services;

namespace GlucoseTrackerWeb.Controllers
{
    public class HomeController : Controller
    {
        private IRepository<Doctor> _doctorRepo;
        private IRepository<Patient> _patientRepo;
        private IRepository<Credentials> _credentialsRepo;

        private static ISession _session;

        public HomeController(IRepository<Doctor> doctorRepo, IRepository<Patient> patientRepo, IRepository<Credentials> credentialsRepo)
        {
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
            _credentialsRepo = credentialsRepo;
        }

        [HttpPost]
        public IActionResult Login(UserCredentials userCreds)
        {
            try
            {
                Doctor doctor = _doctorRepo.Read(d => d.Email == userCreds.Email);
                Credentials creds = _credentialsRepo.Read(c => c.Email == userCreds.Email);

                if (Verify(userCreds.Password, creds.Password))
                {
                    _session = HttpContext.Session;
                    SessionExtensions.SetBool(_session, "LoggedIn", true);
                    return RedirectToAction(Dashboard(doctor));
                }

                TempData["BadLogin"] = true;
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["BadLogin"] = true;
                return RedirectToAction("Index");
            }
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
        public IActionResult Create(UserData userData)
        {
            try
            {
                userData.Doctor.Email = userData.UserCredentials.Email;

                Credentials creds = new Credentials()
                {
                    Email = userData.UserCredentials.Email,
                    Password = HashPassword(userData.UserCredentials.Password),
                    User = userData.Doctor
                };

                userData.Doctor.Token = HashPassword(creds.Password);

                _credentialsRepo.Create(creds);



                //_doctorRepo.Create(userData.Doctor);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(userData);
            }
        }

        [HttpPost]
        public IActionResult Dashboard(Doctor doctor)
        {
            var model = _patientRepo.ReadAll(d => d.UserId == doctor.UserId);

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
