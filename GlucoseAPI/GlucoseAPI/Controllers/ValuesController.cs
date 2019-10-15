using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlucoseAPI.Models.Entities;
using static BCrypt.Net.BCrypt;
using System;

namespace GlucoseAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly GlucoseTrackerContext _context;

        public ValuesController(GlucoseTrackerContext context)
        {
            _context = context;
        }

        [HttpPost("Read")]
        public ActionResult<Patient> GrabPatient(Credentials creds)
        {
            if (VerifyToken(creds.Token))
            {
                try
                {
                    var login = PostPatient(creds).Value;

                    var patient = _context.Patient.Include(p => p.Doctor)
                        .Include(p => p.PatientBloodSugar)
                        .Include(p => p.PatientCarbohydrates)
                        .Include(p => p.PatientExercise)
                        .FirstOrDefault(p => p.PatientId == login.User.UserId);

                    return patient;
                }
                catch (Exception)
                {
                    return Content("Invalid User");
                }
            }
            else
            {
                return Content("Invalid Token");
            }
        }

        [HttpPut("Update")]
        public IActionResult PutPatient(Register register)
        {
            if (VerifyToken(register.Login.Token))
            {
                try
                {
                    Login login = _context.Login.FirstOrDefault(l => l.LoginId == register.Login.LoginId);

                    login.Email = register.Login.Email;
                    login.Password = register.Login.Password;
                    login.Token = register.Login.Token;
                    login.User = register.Patient;

                    _context.Patient.Attach(register.Patient);

                    _context.Entry(register.Patient).State = EntityState.Modified;

                    _context.SaveChanges();

                    return Content("Success");
                }
                catch (Exception)
                {
                    return Content("Invalid User");
                }
            }
            return Content("Invalid Token");
        }

        [HttpPost("Create")]
        public IActionResult CreatePatient(Register register)
        {
            try
            {
                Patient patient = register.Patient;

                if (!(patient.DoctorId is null))
                {
                    patient.Doctor = _context.Doctor.FirstOrDefault(d => d.UserId == patient.DoctorId);
                    patient.Doctor.Patients.Add(patient);
                }
                _context.Patient.Add(patient);

                register.Login.User = register.Patient;

                _context.Login.Add(register.Login);
                _context.SaveChanges();
                return Content("Success");
            }
            catch (Exception)
            {
                return Content("Bad Input");
            }
        }

        private ActionResult<Login> PostPatient(Credentials creds)
        {

            Login login = _context.Login.Include(l => l.User).Where(l => l.Email.Equals(creds.Email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(l => Verify(creds.Password, l.Password));

            return login;
        }


        [HttpDelete ("Delete")]
        public IActionResult DeletePatient(Credentials creds)
        {
            if (VerifyToken(creds.Token))
            {
                try
                {
                    var patient = GrabPatient(creds).Value;

                    _context.Login.Remove(PostPatient(creds).Value);
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
            return Content("Invalid Token");
        }

        private bool VerifyToken(string token)
        {
            
            if(!(_context.Login.FirstOrDefault(l => l.Token == token) is null))
            {
                return true;
            }
            return false;
        }

        private bool UserExists(int id)
        {
            return _context.Patient.Any(e => e.UserId == id);
        }
    }
}
