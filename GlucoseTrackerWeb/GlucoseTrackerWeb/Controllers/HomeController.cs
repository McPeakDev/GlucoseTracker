using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GlucoseAPI.Models.Entities;
using GlucoseAPI.Services;
using static BCrypt.Net.BCrypt;
using SessionExtensions = GlucoseAPI.Services.SessionExtensions;
using GlucoseAPI.Models;

namespace GlucoseTrackerWeb.Controllers
{
    public class HomeController : Controller
    {
        private IRepository<Doctor> _doctorRepo;
        private IRepository<Patient> _patientRepo;
        private IRepository<Auth> _authRepo;
        private IRepository<TokenAuth> _tokenAuthRepo;

        public HomeController(IRepository<Doctor> doctorRepo, IRepository<Patient> patientRepo, IRepository<Auth> authRepo, IRepository<TokenAuth> tokenAuthRepo)
        {
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
            _authRepo = authRepo;
            _tokenAuthRepo = tokenAuthRepo;
        }

        [HttpPost]
        public IActionResult Login(Credentials creds)
        {
            try
            {
                Auth authorization = _authRepo.Read(a => a.Email == creds.Email);
                TokenAuth tokenAuthorization = _tokenAuthRepo.Read(ta => ta.AuthId == authorization.AuthId);
                Doctor doctor = _doctorRepo.Read(d => d.Email == creds.Email);

                if (Verify(creds.Password, authorization.Password))
                {
                    HttpContext.Session.SetString("TokenAuth", tokenAuthorization.Token);

                    return RedirectToAction("Dashboard", "Home");
                    
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
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {

            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                return RedirectToAction("Dashboard", "Home");

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
        public IActionResult Create(DoctorCreationBundle doctorCreationBundle)
        {
            try
            {

                Auth authEntry = new Auth()
                {
                    Email = doctorCreationBundle.Doctor.Email,
                    Password = doctorCreationBundle.Password
                };

                TokenAuth TokenAuthEntry = new TokenAuth()
                {
                    Token = HashPassword(authEntry.Password), 
                    Auth = authEntry,
                    User = doctorCreationBundle.Doctor
                };

                _authRepo.Create(authEntry);
                _tokenAuthRepo.Create(TokenAuthEntry);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(doctorCreationBundle);
            }
        }

        public IActionResult AddPatient()
        {
            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult AddPatient([FromForm(Name = "token")] string token)
        {
            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                try
                {
                    Patient patient = _tokenAuthRepo.Read(ta => ta.Token == token, ta => ta.User).User as Patient;
                    Doctor doctor = _tokenAuthRepo.Read(ta => ta.Token == HttpContext.Session.GetString("TokenAuth"), ta => ta.User).User as Doctor;

                    patient.DoctorId = doctor.UserId;
                    patient.Doctor = doctor;

                    _patientRepo.Update(patient);

                    return RedirectToAction("Dashboard");
                }
                catch (Exception)
                {
                    TempData["BadUser"] = true;
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        public IActionResult RemovePatient()
        {
            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult RemovePatient([FromForm(Name = "id")] int id)
        {
            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                try
                {
                    Patient patient = _patientRepo.Read(p => p.UserId == id);

                    patient.DoctorId = null;
                    patient.Doctor = null;

                    _patientRepo.Update(patient);

                    return RedirectToAction("Dashboard");
                }
                catch (Exception)
                {
                    TempData["BadUser"] = true;
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        public IActionResult Dashboard()
        {

            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                string token = HttpContext.Session.GetString("TokenAuth");

                Doctor doctor = _tokenAuthRepo.Read(ta => ta.Token == token, ta => ta.User).User as Doctor;

                var model = _patientRepo.ReadAll(p => p.PatientBloodSugars, p => p.PatientCarbs, p => p.PatientExercises).Where(p => p.DoctorId == doctor.UserId);
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
