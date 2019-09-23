using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GlucoseTrackerWeb.Models;
using GlucoseTrackerWeb.Services;
using GlucoseTrackerWeb.Models.DBFEntities;
using static BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Http;
using SessionExtensions = GlucoseTrackerWeb.Services.SessionExtensions;

namespace GlucoseTrackerWeb.Controllers
{
    public class HomeController : Controller
    {
        private IDbRepository<User> _userRepo;
        private IDbRepository<Patient> _patientRepo;
        private static ISession _session;

        public HomeController(IDbRepository<User> userRepo)
        {
            _userRepo = userRepo;
          
        }

        [HttpPost]
        public IActionResult Login(Credentials creds)
        {
            User user = _userRepo.Read(creds.Email);

            if (Verify(creds.Password, user.Password))
            {
                _session = HttpContext.Session;
                SessionExtensions.SetBool(_session, "LoggedIn", true);
                return RedirectToAction("Dashboard",user);
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
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                _userRepo.Create(user);
                return RedirectToAction("Index");
            }
            return View(user);
        }
        public IActionResult Dashboard(User user)
        {
            var model = _patientRepo.ReadAll(); ; //= _userRepo.ReadPatients(user.UserId);

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
