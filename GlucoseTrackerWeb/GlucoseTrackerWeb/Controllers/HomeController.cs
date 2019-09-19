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
using static System.Web.HttpUtility;

namespace GlucoseTrackerWeb.Controllers
{
    public class HomeController : Controller
    {
        private IUserRepository _userRepo;

        public HomeController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            return View();
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

        [HttpPost]
        public IActionResult Login(Credentials creds)
        {
            User user = _userRepo.Read(creds.Email);

            if (Verify(creds.Password, user.Password))
            {
                return RedirectToAction("Dashboard");
            }

            TempData["BadLogin"] = true;
            return RedirectToAction("Index");

        }

        public IActionResult Dashboard()
        {
            var model = _userRepo.ReadAll();

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
