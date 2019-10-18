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
        public ActionResult<PatientData> GrabPatientCarbs([FromHeader(Name = "token")]string token)
        {
            try
            {
                PatientData patientData = new PatientData();

                if (VerifyPatient(token))
                {
                    Patient patient = GrabPatient(token).Value;

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
        public ActionResult<StringContent> UpdatePatientCarbs([FromHeader(Name = "token")]string token, PatientData patientData)
        {
            try
            {
                if (VerifyPatient(token))
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
        public ActionResult<StringContent> CreatePatientCarbs([FromHeader(Name = "token")]string token, PatientData patientData)
        {
            try
            {
                if (VerifyPatient(token))
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
        public ActionResult<StringContent> DeletePatientCarbs([FromHeader(Name = "token")]string token, PatientData patientData)
        {
            try
            {
                if (VerifyPatient(token))
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
        public ActionResult<Patient> GrabPatient([FromHeader(Name = "token")]string token)
        {
            try
            {
                Patient patient = _context.TokenAuth.Include(a => a.User).FirstOrDefault(a => a.Token == token).User as Patient;
                

                return patient;
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpPut("Update")]
        public IActionResult PutPatient([FromHeader(Name = "token")]string token, Patient patient)
        {
            try
            {
                if (VerifyPatient(token))
                {
                    _context.Patient.Attach(patient);

                    _context.Entry(patient).State = EntityState.Modified;

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
        public IActionResult CreatePatient(PatientCreationBundle patientCreationBundle)
        {
            try
            {
                Patient patient = patientCreationBundle.Patient;

                Auth authEntry = new Auth()
                {
                    Email = patient.Email,
                    Password = patientCreationBundle.Password
                };

                TokenAuth tokenAuthEntry = new TokenAuth()
                {
                    Token = HashPassword(authEntry.Password)
                };

                tokenAuthEntry.Auth = authEntry;
                tokenAuthEntry.User = patient;

                if (!(patient.DoctorId is null))
                {
                    patient.Doctor = _context.Doctor.FirstOrDefault(d => d.UserId == patient.DoctorId);
                    patient.Doctor.Patients.Add(patient);
                }

                _context.Patient.Add(patient);
                _context.Auth.Add(authEntry);
                _context.TokenAuth.Add(tokenAuthEntry);

                _context.SaveChanges();

                return Content("Patient Created");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        [HttpDelete ("Delete")]
        public IActionResult DeletePatient([FromHeader(Name = "token")]string token)
        {
            try
            {
                if (VerifyPatient(token))
                {

                    Patient patient = GrabPatient(token).Value;
                    TokenAuth tokenAuth = _context.TokenAuth.FirstOrDefault(ta => ta.UserId == patient.UserId);
                    Auth authEntry = _context.Auth.FirstOrDefault(a => a.AuthId == tokenAuth.AuthId);

                    _context.Auth.Remove(authEntry);
                    _context.TokenAuth.Remove(tokenAuth);
                    _context.Patient.Remove(patient);

                    _context.SaveChanges();

                    patient = null;
                    tokenAuth = null;
                    authEntry = null;

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
        public ActionResult<string> GrabToken(Credentials creds)
        {
            try
            {
                Auth authorization = _context.Auth.AsNoTracking().Where(c => c.Email.Equals(creds.Email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(c => Verify(creds.Password, c.Password));

                TokenAuth tokenEntry = _context.TokenAuth.AsNoTracking().FirstOrDefault(t => t.AuthId == authorization.AuthId);

                return tokenEntry.Token;
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
                Patient patient = _context.TokenAuth.AsNoTracking().FirstOrDefault(p => p.Token.Equals(token)).User as Patient;

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