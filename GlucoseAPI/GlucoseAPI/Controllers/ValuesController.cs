using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlucoseAPI.Models.Entities;
using static BCrypt.Net.BCrypt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace GlucoseAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        #region Dependency Injection
        private readonly GlucoseTrackerContext _context;

        public ValuesController(GlucoseTrackerContext context)
        {
            _context = context;
        }
        #endregion

        #region User Properties
        [HttpPost("Carbs")]
        public ActionResult<IQueryable<PatientCarbohydrates>> GrabPatientCarbs(Credentials creds)
        {
            try
            {
                Login login = PostPatient(creds);

                IQueryable<PatientCarbohydrates> patientCarbs = _context.PatientCarbohydrates.Include(pc =>pc.Patient).Where(pc => pc.PatientId == login.User.UserId);

                return new ActionResult<IQueryable<PatientCarbohydrates>>(patientCarbs);
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpPost("BloodSugars")]
        public ActionResult<IQueryable<PatientBloodSugar>> GrabPatientBloodSugars(Credentials creds)
        {
            try
            {
                Login login = PostPatient(creds);

                IQueryable<PatientBloodSugar> patientBloodSugars = _context.PatientBloodSugar.Include(bs => bs.Patient).Where(bs => bs.PatientId == login.User.UserId);

                return new ActionResult<IQueryable<PatientBloodSugar>>(patientBloodSugars);
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpPost("Exercises")]
        public ActionResult<IQueryable<PatientExercise>> GrabPatientExercises(Credentials creds)
        {
            try
            {
                Login login = PostPatient(creds);

                IQueryable<PatientExercise> patientExercises = _context.PatientExercise.Include(pe => pe.Patient).Where(pe => pe.PatientId == login.User.UserId);

                return new ActionResult<IQueryable<PatientExercise>>(patientExercises);
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }
        #endregion

        #region CRUD
        [HttpPost("Read")]
        public ActionResult<Patient> GrabPatient(Credentials creds)
        {
            try
            {
                var login = PostPatient(creds);

                var patient = _context.Patient.Include(p => p.Doctor)
                    .Include(p => p.RecentPatientBloodSugar)
                    .Include(p => p.RecentPatientCarbs)
                    .Include(p => p.RecentPatientExercise)
                    .FirstOrDefault(p => p.PatientId == login.User.UserId);

                return patient;
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpPut("Update")]
        public IActionResult PutPatient(Register register)
        {
            try
            {

                //Login login = PostPatient(register.Credentials);

                _context.Patient.Attach(register.Patient);

                _context.Entry(register.Patient).State = EntityState.Modified;

                _context.SaveChanges();

                return Content("Success");
            }
            catch (Exception)
            {
                return Content("Invalid User/ Credentials");
            }

        }

        [HttpPost("Create")]
        public ActionResult<StringContent> CreatePatient(Register register)
        {
            try
            {
                Login login = new Login()
                {
                    Email = register.Credentials.Email,
                    Password = HashPassword(register.Credentials.Password),
                    Token = HashPassword(HashPassword(register.Credentials.Password))
                };

                Patient patient = register.Patient;

                if (!(patient.DoctorId is null))
                {
                    patient.Doctor = _context.Doctor.FirstOrDefault(d => d.UserId == patient.DoctorId);
                    patient.Doctor.Patients.Add(patient);
                }
                _context.Patient.Add(patient);

                login.User = register.Patient;

                _context.Login.Add(login);
                _context.SaveChanges();

                return (ActionResult<StringContent>) new StringContent(JObject.FromObject(login).ToString(), Encoding.UTF8, "application/json");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpDelete ("Delete")]
        public IActionResult DeletePatient(Credentials creds)
        {
            try
            {
                Login login = PostPatient(creds);

                var patient = GrabPatient(creds).Value;

                _context.Login.Remove(login);
                _context.Patient.Remove(patient);

                _context.SaveChanges();

                patient = null;

                return Content("Deleted!");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }
        #endregion

        #region Helper Methods
        private Login PostPatient(Credentials creds)
        {

            Login login = _context.Login.Include(l => l.User).Where(l => l.Email.Equals(creds.Email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(l => Verify(creds.Password, l.Password));

            return login;
        }

        private bool UserExists(int id)
        {
            return _context.Patient.Any(e => e.UserId == id);
        }
        #endregion
    }
}