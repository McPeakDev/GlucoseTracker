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
        [HttpPost("ReadData")]
        public ActionResult<PatientData> GrabPatientCarbs(UserCredentials userCreds)
        {
            try
            {
                PatientData patientData = new PatientData();

                if (VerifyPatient(userCreds.Token))
                {
                    Patient patient = GrabPatient(userCreds).Value;

                    patientData.UserCredentials = userCreds;

                    patientData.PatientCarbohydrates = _context.PatientCarbohydrates.Include(pc => pc.Patient).Where(pc => pc.UserId == patient.UserId).ToList();
                    patientData.PatientExercises = _context.PatientExercise.Include(pe => pe.Patient).Where(pe => pe.UserId == patient.UserId).ToList();
                    patientData.PatientBloodSugars = _context.PatientBloodSugar.Include(bs=> bs.Patient).Where(bs => bs.UserId == patient.UserId).ToList();

                    return patientData;
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpPut("UpdateData")]
        public ActionResult<StringContent> UpdatePatientCarbs(PatientData patientData)
        {
            try
            {
                if (VerifyPatient(patientData.UserCredentials.Token))
                {
                    foreach (var bs in patientData.PatientBloodSugars)
                    {
                        _context.PatientBloodSugar.Attach(bs);
                        _context.Entry(bs).State = EntityState.Modified;
                    }


                    foreach (var pe in patientData.PatientExercises)
                    {
                        _context.PatientExercise.Attach(pe);
                        _context.Entry(pe).State = EntityState.Modified;
                    }

                    foreach (var pc in patientData.PatientCarbohydrates)
                    {
                        _context.PatientCarbohydrates.Attach(pc);
                        _context.Entry(pc).State = EntityState.Modified;
                    }

                    _context.SaveChanges();

                    return Content("Patient Data Updated");
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpPost("CreateData")]
        public ActionResult<StringContent> CreatePatientCarbs(PatientData patientData)
        {
            try
            {
                if (VerifyPatient(patientData.UserCredentials.Token))
                {
                    foreach (var bs in patientData.PatientBloodSugars)
                    {
                        _context.PatientBloodSugar.Add(bs);
                    }

                    foreach (var pe in patientData.PatientExercises)
                    {
                        _context.PatientExercise.Add(pe);
                    }

                    foreach (var pc in patientData.PatientCarbohydrates)
                    {
                        _context.PatientCarbohydrates.Add(pc);
                    }

                    _context.SaveChanges();

                    return Content("Patient Data Created");
                }
                return Content("Invlaid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpDelete("DeleteData")]
        public ActionResult<StringContent> DeletePatientCarbs(PatientData patientData)
        {
            try
            {
                if (VerifyPatient(patientData.UserCredentials.Token))
                {
                    foreach (var bs in patientData.PatientBloodSugars)
                    {
                        _context.PatientBloodSugar.Remove(bs);
                    }

                    foreach (var pe in patientData.PatientExercises)
                    {
                        _context.PatientExercise.Remove(pe);
                    }

                    foreach (var pc in patientData.PatientCarbohydrates)
                    {
                        _context.PatientCarbohydrates.Remove(pc);
                    }

                    _context.SaveChanges();

                    return Content("Patient Data Deleted");
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
                var patient = _context.Patient.Include(p => p.Doctor)
                    .Include(p => p.PatientBloodSugars)
                    .Include(p => p.PatientCarbs)
                    .Include(p => p.PatientExercises)
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
                    userData.Patient.UserId = _context.Credentials.Where(c => c.Email.Equals(userData.UserCredentials.Email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(c => Verify(userData.UserCredentials.Password, c.Password)).UserId;

                    _context.Patient.Attach(userData.Patient);

                    _context.Entry(userData.Patient).State = EntityState.Modified;

                    _context.SaveChanges();

                    return Content("Success");
                }
                return Content("Patient Updated");
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

                return Content("Patient Created");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpDelete ("Delete")]
        public IActionResult DeletePatient(UserCredentials userCredentials)
        {
            try
            {
                if (VerifyPatient(userCredentials.Token))
                {

                    Patient patient = GrabPatient(userCredentials).Value;
                    Credentials creds = _context.Credentials.FirstOrDefault(c => c.UserId == patient.UserId);

                    _context.Credentials.Remove(creds);
                    _context.Patient.Remove(patient);

                    _context.SaveChanges();

                    patient = null;
                    creds = null;

                    return Content("Patient Deleted");
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }
        #endregion

        #region Login
        [HttpPost("Token")]
        public ActionResult<UserCredentials> GrabToken(UserCredentials userCreds)
        {
            try
            {
                Credentials creds = _context.Credentials.AsNoTracking().Where(c => c.Email.Equals(userCreds.Email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(c => Verify(userCreds.Password, c.Password));

                Patient patient = _context.Patient.AsNoTracking().FirstOrDefault(p => p.UserId == creds.UserId);

                userCreds.Token = patient.Token;

                return userCreds;
            }
            catch (Exception)
            {
                return Content("Invalid Credentials");
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