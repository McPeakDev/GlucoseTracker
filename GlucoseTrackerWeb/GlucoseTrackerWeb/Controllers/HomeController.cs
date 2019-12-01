///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerWeb/GlucoseTrackerWeb
//	File Name:         HomeController.cs
//	Description:       Controller for all Doctor Related Tasks
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GlucoseAPI.Models.Entities;
using GlucoseAPI.Services;
using static BCrypt.Net.BCrypt;
using GlucoseAPI.Models;

namespace GlucoseTrackerWeb.Controllers
{
    /// <summary>
    /// Controller for all Doctor Related Tasks
    /// </summary>
    public class HomeController : Controller
    {
        #region Dependency Injection
        private readonly IRepository<Doctor> _doctorRepo;
        private readonly IRepository<Patient> _patientRepo;
        private readonly IRepository<Auth> _authRepo;
        private readonly IRepository<TokenAuth> _tokenAuthRepo;

        public HomeController(IRepository<Doctor> doctorRepo, IRepository<Patient> patientRepo, IRepository<Auth> authRepo, IRepository<TokenAuth> tokenAuthRepo)
        {
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
            _authRepo = authRepo;
            _tokenAuthRepo = tokenAuthRepo;
        }
        #endregion

        #region Auth/DeAuth
        /// <summary>
        /// Authentication for Doctors
        /// </summary>
        /// <param name="creds">A Doctor's Credentials</param>
        /// <returns>A page based upon auth status</returns>
        [HttpPost]
        public IActionResult Login(Credentials creds)
        {
            //Try to Authenticate
            try
            {
                //Find Authorization based up on Credentials
                Auth authorization = _authRepo.Read(a => a.Email == creds.Email);
                TokenAuth tokenAuthorization = _tokenAuthRepo.Read(ta => ta.AuthId == authorization.AuthId);

                //Read the Doctor
                Doctor doctor = _doctorRepo.Read(d => d.Email == creds.Email);

                //If the doctor exists and is authed.
                if (!(doctor is null) && Verify(creds.Password, authorization.Password))
                {
                    //Set a Session
                    HttpContext.Session.SetString("TokenAuth", tokenAuthorization.Token);

                    //Redirect to the Landing Page
                    return RedirectToAction("Dashboard", "Home");
                    
                }

                //Otherwise.. Assign A Temp Var called BadLogin
                TempData["BadLogin"] = true;

                //return the Login Page.
                return View("Index");
            }
            catch (Exception)
            {
                //Otherwise.. Assign A Temp Var called BadLogin
                TempData["BadLogin"] = true;

                //return the Login Page.
                return View("Index");
            }
        }

        /// <summary>
        /// DeAuth for Doctors
        /// </summary>
        /// <returns>The Login Page</returns>
        public IActionResult Logout()
        {
            //Empty Session and Redirect to Login.
            HttpContext.Session.Clear();
            Response.Cookies.Delete("TokenAuth");
            return RedirectToAction("Index");
        }
        #endregion

        #region Login Page
        /// <summary>
        /// Login Page.
        /// </summary>
        /// <returns>A View based upon login status</returns>
        public IActionResult Index()
        {
            //If the Doctor is Logged-In
            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                //Display Landing Page
                return RedirectToAction("Dashboard", "Home");

            }
            else
            {
                //Display
                return View();
            }
        }
        #endregion

        #region Registration 
        /// <summary>
        /// Create View for Doctor.
        /// </summary>
        /// <returns>The Create Doctor Form</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new Doctor
        /// </summary>
        /// <param name="doctorCreationBundle">A Doctor Creation Bundle</param>
        /// <returns>The Index Page upon Success</returns>
        [HttpPost]
        public IActionResult Create(DoctorCreationBundle doctorCreationBundle)
        {
            //If the Model is valid.
            if (ModelState.IsValid)
            {
                //Try to create a doctor
                try
                {
                    //Create new Auth Entries.
                    Auth authEntry = new Auth()
                    {
                        Email = doctorCreationBundle.Doctor.Email,
                        Password = HashPassword(doctorCreationBundle.Password)
                    };

                    TokenAuth TokenAuthEntry = new TokenAuth()
                    {
                        Token = HashPassword(authEntry.Password),
                        Auth = authEntry,
                        User = doctorCreationBundle.Doctor
                    };

                    _authRepo.Create(authEntry);
                    _tokenAuthRepo.Create(TokenAuthEntry);

                    //Redirect to Login
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    if(_doctorRepo.ReadAll().ToList().Any(d => d.Email == doctorCreationBundle.Doctor.Email))
                    {
                        TempData["UserExists"] = true;
                    }
                    //Otherwise... Return the Create view.
                    return View(doctorCreationBundle);
                }
            }
            else
            {
                //Otherwise... Return the Create view
                return View(doctorCreationBundle);
            }
        }
        #endregion

        #region Assign Patient
        /// <summary>
        /// Add a Patient View
        /// </summary>
        /// <returns>The Add a Patient View</returns>
        public IActionResult AddPatient()
        {
            //Verify the Doctor is Logged In
            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                //Return the View.
                return View();
            }
            else
            {
                //Otehrwise... Redirect to Login
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Adds a Pattient to a Doctor
        /// </summary>
        /// <param name="token">A Patient's token</param>
        /// <returns>The Dashboard View Upon Success</returns>
        [HttpPost]
        public IActionResult AddPatient([FromForm(Name = "token")] string token)
        {
            //If the Doctor is Logged-In
            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                //Try to add a Patient
                try
                {
                    //Query the Patient
                    Patient patient = _tokenAuthRepo.Read(ta => ta.Token.Substring(ta.Token.Length -6, 6) == token , ta => ta.User).User as Patient;

                    //Query the Doctor
                    Doctor doctor = _tokenAuthRepo.Read(ta => ta.Token == HttpContext.Session.GetString("TokenAuth"), ta => ta.User).User as Doctor;

                    //Assign the Doctor to the Patient
                    patient.DoctorId = doctor.UserId;
                    patient.Doctor = doctor;

                    //Update the Patient
                    _patientRepo.Update(patient);

                    //Redirect to Landing Page
                    return RedirectToAction("Dashboard");
                }
                catch (Exception)
                {
                    //Otherwise.. Invalid Token and Display and Error
                    TempData["BadUser"] = true;
                    return View();
                }
            }
            else
            {
                //Otehrwise... Return to Login 
                return RedirectToAction("Index");
            }

        }
        #endregion

        #region Remove Patient

        /// <summary>
        /// Removes a pateint from a doctor
        /// </summary>
        /// <param name="id">A Patient Id</param>
        /// <returns>The Dashboard View</returns>
        [HttpPost]
        public IActionResult RemovePatient([FromForm(Name = "id")] int id)
        {
            //If the Doctor is Logged-In
            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                //Try to Remove the Patient
                try
                {
                    //Find the Patient
                    Patient patient = _patientRepo.Read(p => p.UserId == id);

                    //Remove the Doctor from the Patient
                    patient.DoctorId = null;
                    patient.Doctor = null;

                    //Update the Patient
                    _patientRepo.Update(patient);

                    //Return the Landing Page
                    return RedirectToAction("Dashboard");
                }
                catch (Exception)
                {
                    //Otherwise... Set an Error and return the landing page. 
                    TempData["BadUser"] = true;
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                //Otherwise.. Redirect to the Login Page. 
                return RedirectToAction("Index");
            }

        }
        #endregion

        #region Landing Page
        /// <summary>
        /// The Landing Page. Holds Patients that are assigned to the doctor.
        /// </summary>
        /// <returns>The Landing Page</returns>
        public IActionResult Dashboard()
        {
            //If the Doctor is Logged-In
            if (!(HttpContext.Session.GetString("TokenAuth") is null))
            {
                //Assign token to the Doctor's token
                string token = HttpContext.Session.GetString("TokenAuth");

                //Find the Doctor
                Doctor doctor = _tokenAuthRepo.Read(ta => ta.Token == token, ta => ta.User).User as Doctor;

                //Read All Patients, for the current doctor.
                var model = _patientRepo.ReadAll(p => p.PatientBloodSugars, p => p.PatientCarbs, p => p.PatientExercises).Where(p => p.DoctorId == doctor.UserId);
                
                //Return the landing page.
                return View(model);
            }
            else
            {
                //Otherwise... Redirect to the Login Page.
                return RedirectToAction("Index");
            }

        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
