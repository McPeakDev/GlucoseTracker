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
        public ActionResult<IQueryable<PatientCarbohydrates>> GrabPatientCarbs(UserCredentials userCreds)
        {
            try
            {
                if (VerifyPatient(userCreds.Token))
                {
                    Patient patient = GrabPatient(userCreds).Value;

                    IQueryable<PatientCarbohydrates> patientCarbs = _context.PatientCarbohydrates.Include(pc => pc.Patient).Where(pc => pc.PatientId == patient.UserId);

                    return new ActionResult<IQueryable<PatientCarbohydrates>>(patientCarbs);
                }
                return Content("Invlaid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }
        #endregion

        #region CRUD
        [HttpPost("Read")]
        public ActionResult<Patient> GrabPatient(UserCredentials userCreds)
        {
            try
            {
                var patient = _context.Patient.AsNoTracking().Include(p => p.Doctor)
                    .Include(p => p.RecentPatientBloodSugar)
                    .Include(p => p.RecentPatientCarbs)
                    .Include(p => p.RecentPatientExercise)
                    .Where(p => p.Email.Equals(userCreds.Email, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault(p => p.Token == userCreds.Token);

                return patient;
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpPut("Update")]
        public IActionResult PutPatient(UserData userData)
        {
            try
            {
                if (VerifyPatient(userData.Token))
                {
                    Patient patient = GrabPatient(userData.UserCredentials).Value;

                    if (patient.UserId == userData.Patient.UserId)
                    {

                        _context.Patient.Attach(userData.Patient);

                        _context.Entry(userData.Patient).State = EntityState.Modified;

                        _context.SaveChanges();

                        return Content("Success");
                    }
                    throw new Exception();
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }

        }

        [HttpPost("Create")]
        public ActionResult<StringContent> CreatePatient(UserData userData)
        {
            try
            {
                Credentials credentials = new Credentials()
                {
                    UserId = userData.Patient.UserId,
                    Email = userData.UserCredentials.Email,
                    Password = HashPassword(userData.UserCredentials.Password),
                    User = userData.Patient
                };

                Patient patient = userData.Patient;
                patient.Token = HashPassword(credentials.Password);

                if (!(patient.DoctorId is null))
                {
                    patient.Doctor = _context.Doctor.FirstOrDefault(d => d.UserId == patient.DoctorId);
                    patient.Doctor.Patients.Add(patient);
                }
                _context.Patient.Add(patient);
                _context.Credentials.Add(credentials);

                _context.SaveChanges();

                return Content(patient.Token);
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpDelete ("Delete")]
        public IActionResult DeletePatient(UserData userData)
        {
            try
            {
                if (VerifyPatient(userData.Token))
                {

                    Patient patient = GrabPatient(userData.UserCredentials).Value;
                    Credentials creds = _context.Credentials.FirstOrDefault(c => c.UserId == patient.UserId);

                    _context.Credentials.Remove(creds);
                    _context.Patient.Remove(patient);

                    _context.SaveChanges();

                    patient = null;
                    creds = null;

                    return Content("Deleted!");
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }
        #endregion

        #region Helper Methods
        private bool VerifyPatient(string token)
        {
            try
            {
                Patient patient = _context.Patient.AsNoTracking().FirstOrDefault(p => p.Token.Equals(token));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool UserExists(int id)
        {
            return _context.Patient.Any(e => e.UserId == id);
        }
        #endregion
    }
}